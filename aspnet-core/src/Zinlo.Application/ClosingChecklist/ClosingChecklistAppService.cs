using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Zinlo.Tasks.Dtos;
using Zinlo.Comment;
using Zinlo.Comment.Dtos;
using Zinlo.ClosingChecklist.Dtos;
using System.Collections.Generic;
using Abp.UI;
using Zinlo.Authorization.Users;
using Zinlo.Attachments;
using Zinlo.Attachments.Dtos;
using Zinlo.Authorization.Users.Profile;
using NUglify.Helpers;
using Zinlo.TimeManagements;
using Zinlo.TimeManagements.Dto;

namespace Zinlo.ClosingChecklist
{
    public class ClosingChecklistAppService : ZinloAppServiceBase, IClosingChecklistAppService
    {
        #region|Properties|
        private readonly IRepository<ClosingChecklist, long> _closingChecklistRepository;
        private readonly ICommentAppService _commentAppService;
        private readonly IRepository<User, long> _userRepository;
        private readonly IAttachmentAppService _attachmentAppService;
        private readonly IProfileAppService _profileAppService;
        private readonly ITimeManagementsAppService _managementsAppService;

        #endregion
        #region|#Constructor Dependencies|
        public ClosingChecklistAppService(IProfileAppService profileAppService,
                                          IRepository<ClosingChecklist, long> closingChecklistRepository,
                                          UserManager userManager, ICommentAppService commentAppService,
                                          IRepository<User, long> userRepository,
                                          IAttachmentAppService attachmentAppService, ITimeManagementsAppService managementsAppService)
        {
            _closingChecklistRepository = closingChecklistRepository;
            _commentAppService = commentAppService;
            _userRepository = userRepository;
            _attachmentAppService = attachmentAppService;
            _managementsAppService = managementsAppService;
            _profileAppService = profileAppService;
        }
        #endregion
        #region|Get All|
        public async Task<PagedResultDto<TasksGroup>> GetAll(GetAllClosingCheckListInput input)
        {

            if (!(input.DateFilter != null /*&& input.DateFilter < DateTime.Now.AddMonths(1)*/)) return new PagedResultDto<TasksGroup>();
            var query = _closingChecklistRepository.GetAll().Where(e => e.ClosingMonth.Month == input.DateFilter.Value.Month && e.ClosingMonth.Year == input.DateFilter.Value.Year).Include(rest => rest.Category).Include(u => u.Assignee)
                                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TaskName.Contains(input.Filter))
                                .WhereIf(input.StatusFilter != 0, e => false || e.Status == (Zinlo.ClosingChecklist.Status)input.StatusFilter)
                                .WhereIf(input.CategoryFilter != 0, e => false || e.CategoryId == input.CategoryFilter)
                                .WhereIf(input.AssigneeId != 0, e => false || e.AssigneeId == input.AssigneeId);
            var status = await GetMonthStatus((DateTime)input.DateFilter);
            var pagedAndFilteredTasks = query.OrderBy(input.Sorting ?? "DueDate asc").PageBy(input);
            var totalCount = query.Count();
            var getUserWithPictures = (from o in query.ToList()

                                       select new GetUserWithPicture()
                                       {
                                           Id = o.AssigneeId,
                                           Name = o.Assignee.FullName,
                                           Picture = o.Assignee.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)o.Assignee.ProfilePictureId).Result.ProfilePicture : ""
                                       }).ToList();

            getUserWithPictures = getUserWithPictures.DistinctBy(p => new { p.Id, p.Name }).ToList();

            var closingCheckList = from o in pagedAndFilteredTasks.ToList()

                                   select new ClosingCheckListForViewDto()
                                   {
                                       Id = o.Id,
                                       AssigneeId = o.AssigneeId,
                                       StatusId = (int)o.Status,
                                       AssigniName = o.Assignee.FullName,
                                       TaskName = o.TaskName,
                                       Status = o.Status.ToString(),
                                       Category = o.Category.Title,
                                       DueDate = o.DueDate,
                                       ProfilePicture = o.Assignee.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)o.Assignee.ProfilePictureId).Result.ProfilePicture : ""
                                   };

            var result = closingCheckList.ToList();
            var response = result.GroupBy(x => x.DueDate.Date).Select(x => new TasksGroup
            {
                MonthStatus = status,
                OverallMonthlyAssignee = getUserWithPictures,
                DueDate = x.Key,
                Group = x.Select(y => new ClosingCheckListForViewDto
                {
                    DueDate = y.DueDate,
                    AssigneeId = y.AssigneeId,
                    Category = y.Category,
                    StatusId = y.StatusId,
                    Id = y.Id,
                    AssigniName = y.AssigniName,
                    Status = y.Status,
                    TaskName = y.TaskName,
                    ProfilePicture = y.ProfilePicture,
                }
                )
            });


            return new PagedResultDto<TasksGroup>(
                totalCount,
                response.ToList()
            );
        }
        #endregion
        #region|Create Eidt Details Delete|
        public async Task CreateOrEdit(CreateOrEditClosingChecklistDto input)
        {
            if (input.Frequency != FrequencyDto.None && input.ClosingMonth > input.EndsOn) throw new UserFriendlyException(L("ClosingMonthGreaterException"));
            if (input.Id == 0)
            {
                await TaskIteration(input);
            }
            else
            {
                var currentTaskDetail = await _closingChecklistRepository.FirstOrDefaultAsync(p => p.Id == input.Id);
                if (currentTaskDetail.Frequency == (Frequency)input.Frequency && currentTaskDetail.EndsOn.GetValueOrDefault().Year == input.EndsOn.Year && currentTaskDetail.EndsOn.GetValueOrDefault().Month == input.EndsOn.Month)
                {
                    if (!(currentTaskDetail.EndOfMonth == input.EndOfMonth && currentTaskDetail.DueOn == input.DueOn &&
                         currentTaskDetail.DayBeforeAfter == (DaysBeforeAfter)input.DayBeforeAfter))
                    {
                        input.DueDate = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);
                    }
                    ObjectMapper.Map(input, currentTaskDetail);
                    await CreateComment(input.CommentBody, input.Id);
                }
                else
                {
                    var taskList = _closingChecklistRepository.GetAll().Where(p => p.GroupId == input.GroupId).ToList();
                    foreach (var task in taskList)
                    {
                        var checkManagementExist = await _managementsAppService.CheckManagementExist(task.ClosingMonth);
                        if (!checkManagementExist)
                        {
                            var managementDto = new CreateOrEditTimeManagementDto
                            {
                                Month = task.ClosingMonth,
                                Status = false
                            };
                            await _managementsAppService.CreateOrEdit(managementDto);
                        }
                    }
                    var managementDetail = _managementsAppService.GetOpenManagement();
                    foreach (var item in managementDetail)
                    {
                        var task = taskList.FirstOrDefault(p => p.ClosingMonth.Year == item.Month.Year && p.ClosingMonth.Month == item.Month.Month);
                        if (task != null) _closingChecklistRepository.Delete(task);
                        CurrentUnitOfWork.SaveChanges();
                    }

                    await TaskIteration(input);
                }
            }
        }

        private async Task TaskIteration(CreateOrEditClosingChecklistDto input)
        {
            Guid? oldGroupId = null;
            var forEdit = false;
            if (input.Id == 0)
            {
                input.ClosingMonth = input.ClosingMonth.AddDays(1);
                input.EndsOn = input.EndsOn.AddDays(1);
                input.GroupId = Guid.NewGuid();
            }
            else
            {
                oldGroupId = input.GroupId;
                forEdit = true;
            }
            if (input.EndOfMonth) input.DueOn = 0;
            switch (input.Frequency)
            {
                case FrequencyDto.None:
                    {
                        input.DueDate = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);

                        if (input.Id == 0 || (!await CheckTaskExist(input.ClosingMonth, (Guid)oldGroupId)))
                        {
                            if (forEdit) input.Id = 0;
                            await Create(input);
                        }
                        break;
                    }
                case FrequencyDto.Monthly:
                    {
                        var monthDifference = GetMonthDifference(input.ClosingMonth, input.EndsOn);
                        for (int i = 1; i <= monthDifference; i++)
                        {
                            input.DueDate = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth,
                                input.DueOn, input.EndOfMonth);
                            if (input.Id == 0 || (!await CheckTaskExist(input.ClosingMonth, (Guid)input.GroupId)))
                            {
                                if (forEdit) input.Id = 0;
                                await Create(input);
                            }
                            var nextMonth = input.ClosingMonth.AddMonths(1);
                            input.ClosingMonth = nextMonth;

                        }

                        break;
                    }
                case FrequencyDto.Quarterly:
                    {
                        var monthDifference = GetMonthDifference(input.ClosingMonth, input.EndsOn);
                        for (int i = 1; i <= monthDifference; i += 3)
                        {
                            input.DueDate = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth,
                                input.DueOn, input.EndOfMonth);
                            if (input.Id == 0 || (!await CheckTaskExist(input.ClosingMonth, (Guid)input.GroupId)))
                            {
                                if (forEdit) input.Id = 0;
                                await Create(input);
                            }
                            var nextMonth = input.ClosingMonth.AddMonths(3);
                            input.ClosingMonth = nextMonth;

                        }

                        break;
                    }
                case FrequencyDto.Annually:
                    {
                        var yearsDifference = GetDifferenceInYears(input.ClosingMonth, input.EndsOn);
                        for (int i = 0; i <= yearsDifference; i++)
                        {
                            input.DueDate = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth,
                                input.DueOn, input.EndOfMonth);
                            if (input.Id == 0 || (!await CheckTaskExist(input.ClosingMonth, (Guid)input.GroupId)))
                            {
                                if (forEdit) input.Id = 0;
                                await Create(input);
                            }
                            var nextMonth = input.ClosingMonth.AddYears(1);
                            input.ClosingMonth = nextMonth;

                        }

                        break;
                    }
                case FrequencyDto.XNumberOfMonths:
                    {
                        var monthDifference = GetMonthDifference(input.ClosingMonth, input.EndsOn);
                        for (int i = 1; i <= monthDifference; i += input.NoOfMonths)
                        {
                            input.DueDate = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth,
                                input.DueOn, input.EndOfMonth);
                            if (input.Id == 0 || (!await CheckTaskExist(input.ClosingMonth, (Guid)input.GroupId)))
                            {
                                if (forEdit) input.Id = 0;
                                await Create(input);
                            }
                            var nextMonth = input.ClosingMonth.AddMonths(input.NoOfMonths);
                            input.ClosingMonth = nextMonth;

                        }

                        break;
                    }
            }
        }

        private static int GetMonthDifference(DateTime start, DateTime end)
        {
            return ((end.Year * 12 + end.Month) - (start.Year * 12 + start.Month)) + 1;
        }
        private static int GetDifferenceInYears(DateTime startDate, DateTime endDate)
        {
            int years = endDate.Year - startDate.Year;

            if (startDate.Month == endDate.Month && endDate.Day < startDate.Day
                || endDate.Month < startDate.Month)
            {
                years--;
            }

            return years;
        }
        protected virtual async Task Create(CreateOrEditClosingChecklistDto input)
        {

            var task = ObjectMapper.Map<ClosingChecklist>(input);
            if (AbpSession.TenantId != null)
            {
                task.TenantId = (int)AbpSession.TenantId;
            }
            var checklistId = await _closingChecklistRepository.InsertAndGetIdAsync(task);

            await CreateComment(input.CommentBody, checklistId);

            if (input.AttachmentsPath != null)
            {
                PostAttachmentsPathDto postAttachmentsPathDto = new PostAttachmentsPathDto
                {
                    FilePath = input.AttachmentsPath,
                    TypeId = checklistId,
                    Type = 1
                };
                await _attachmentAppService.PostAttachmentsPath(postAttachmentsPathDto);
            }
        }
        private async Task CreateComment(string commentBody, long taskId)
        {
            if (!String.IsNullOrWhiteSpace(commentBody))
            {
                var commentDto = new CreateOrEditCommentDto()
                {
                    Body = commentBody,
                    Type = CommentTypeDto.ClosingChecklist,
                    TypeId = taskId
                };
                await _commentAppService.Create(commentDto);
            }
        }
        public async Task<GetTaskForEditDto> GetTaskForEdit(long id)
        {
            var task = await _closingChecklistRepository.GetAll().Include(a => a.Assignee).Include(a => a.Category).FirstOrDefaultAsync(x => x.Id == id);
            var output = ObjectMapper.Map<GetTaskForEditDto>(task);
            output.MonthStatus = await GetMonthStatus(task.ClosingMonth);
            output.Comments = await _commentAppService.GetComments((int)CommentTypeDto.ClosingChecklist, id);
            output.Attachments = await _attachmentAppService.GetAttachmentsPath(task.Id, 1);
            output.ProfilePicture = task.Assignee.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)task.Assignee.ProfilePictureId).Result.ProfilePicture : "";
            return output;
        }
        public async Task<DetailsClosingCheckListDto> GetDetails(long id)
        {
            var task = _closingChecklistRepository.GetAll().Include(u => u.Assignee).Include(c => c.Category).FirstOrDefault(x => x.Id == id);
            var output = ObjectMapper.Map<DetailsClosingCheckListDto>(task);
            if (task != null)
            {
                output.AssigneeName = task.Assignee.Name;
                output.TaskStatus = task.Status.ToString();
                output.Status = (StatusDto)task.Status;
                output.CategoryName = task.Category.Title;
                output.CategoryId = task.CategoryId;
                output.Comments = await _commentAppService.GetComments((int)CommentTypeDto.ClosingChecklist, task.Id);
                output.Attachments = await _attachmentAppService.GetAttachmentsPath(task.Id, 1);
                output.ProfilePicture = task.Assignee.ProfilePictureId.HasValue
                    ? "data:image/jpeg;base64," + _profileAppService
                          .GetProfilePictureById((Guid)task.Assignee.ProfilePictureId).Result.ProfilePicture
                    : "";
                output.MonthStatus = await GetMonthStatus(task.ClosingMonth);
                output.DueDate = task.DueDate;
            }

            return output;
        }
        public async Task Delete(long id)
        {
            await _closingChecklistRepository.DeleteAsync(id);
        }
        #endregion
        #region|Helpers|
        public async Task<List<GetUserWithPicture>> GetUserWithPicture(string searchTerm, long? id)
        {
            List<User> list = await _userRepository.GetAll().ToListAsync();
            list = !String.IsNullOrEmpty(searchTerm) ? list.Where(x => x.FullName.ToLower().Contains(searchTerm.Trim().ToLower())).ToList() : list.Where(x => x.Id == id).ToList();
            var query = (from o in list
                         select new GetUserWithPicture()
                         {
                             Name = o.FullName,
                             Id = o.Id,
                             Picture = o.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)o.ProfilePictureId).Result.ProfilePicture : ""
                         }).ToList();
            var assets = query;
            return assets;
        }
        public async Task ChangeAssignee(ChangeAssigneeDto changeAssigneeDto)
        {
            var task = await _closingChecklistRepository.FirstOrDefaultAsync(changeAssigneeDto.TaskId);
            if (task != null)
            {
                task.AssigneeId = changeAssigneeDto.AssigneeId;
                _closingChecklistRepository.Update(task);
            }
        }
        public async Task ChangeStatus(ChangeStatusDto changeStatusDto)
        {
            var task = await _closingChecklistRepository.FirstOrDefaultAsync(changeStatusDto.TaskId);
            if (task != null)
            {
                task.Status = (Status)changeStatusDto.StatusId;
                _closingChecklistRepository.Update(task);
            }
        }
        public List<NameValueDto<string>> GetCurrentMonthDays(DateTime dateTime)
        {
            if (dateTime == DateTime.MinValue)
            {
                var list = new List<NameValueDto<string>>();
                var now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var nameValueDto = new NameValueDto<string>
                    {
                        Value = date.Day.ToString(),
                        Name = date.Day.ToString()
                    };
                    list.Add(nameValueDto);
                }
                return list;

            }
            else
            {
                var list = new List<NameValueDto<string>>();
                var now = dateTime; //  DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var nameValueDto = new NameValueDto<string>
                    {
                        Value = date.Day.ToString(),
                        Name = date.Day.ToString()
                    };
                    list.Add(nameValueDto);
                }
                return list;

            }


        }
        public string GenerateGroupId(long id, int iteration)
        {
            return id + "-" + iteration;
        }
        #endregion
        #region|Frequency Logic for Create Task|

        public int GetTotalDaysByMonth(DateTime dateTime)
        {
            return DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
        }
        public DateTime GetNextIterationDateAfterDueDate(DaysBeforeAfterDto daysBeforeAfter, DateTime closingMonth, int numberOfDays, bool endsOfMonth)
        {
            var nextDate = closingMonth;
            if (endsOfMonth)
            {
                nextDate = new DateTime(closingMonth.Year, closingMonth.Month, (GetTotalDaysByMonth(closingMonth)));
            }
            else if (daysBeforeAfter == DaysBeforeAfterDto.DaysBefore)
            {
                nextDate = new DateTime(closingMonth.Year, closingMonth.Month, (GetTotalDaysByMonth(closingMonth) - numberOfDays));
            }
            else
            {
                nextDate = nextDate.AddDays(-nextDate.Day);
                var number = GetTotalDaysByMonth(closingMonth) + numberOfDays;
                nextDate = nextDate.AddDays(number);

            }
            return nextDate;

        }
        protected virtual async Task<bool> GetMonthStatus(DateTime dateTime)
        {
            return await _managementsAppService.GetMonthStatus(dateTime);
        }

        private async Task<bool> CheckTaskExist(DateTime closingMonth, Guid groupId)
        {
            var ifTaskExist = await _closingChecklistRepository.FirstOrDefaultAsync(p =>
                p.GroupId == groupId && p.ClosingMonth.Month == closingMonth.Month &&
                p.ClosingMonth.Year == closingMonth.Year);
            if (ifTaskExist != null) return true;
            return false;
        }
        #endregion
    }
}

