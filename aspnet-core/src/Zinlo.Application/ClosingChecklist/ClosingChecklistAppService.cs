using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Threading.Tasks;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Zinlo.Authorization;
using Zinlo.Tasks.Dtos;
using Zinlo.Comment;
using Zinlo.Comment.Dtos;
using Zinlo.ClosingChecklist.Dtos;
using System.Collections.Generic;
using Bogus.Extensions;
using Zinlo.Authorization.Users;
using Zinlo.Attachments;
using Zinlo.Attachments.Dtos;
using Zinlo.Authorization.Users.Profile;
using NUglify.Helpers;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Zinlo.TimeManagements;

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
        #region|Global Parameters|
        public bool Flag = true;
        public DateTime IterationStartClosingMonth = DateTime.MinValue;
        public bool IsIterationFrequencyChanged = false;
        public DateTime EditedTaskClosingMonth = DateTime.MinValue;
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
            var query = _closingChecklistRepository.GetAll().Where(e => e.DueDate.Month == input.DateFilter.Value.Month && e.ClosingMonth.Year == input.DateFilter.Value.Year).Include(rest => rest.Category).Include(u => u.Assignee)
                                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TaskName.Contains(input.Filter))
                                .WhereIf(input.StatusFilter != 0, e => false || e.Status == (Zinlo.ClosingChecklist.Status)input.StatusFilter)
                                .WhereIf(input.CategoryFilter != 0, e => false || e.CategoryId == input.CategoryFilter)
                                .WhereIf(input.AssigneeId != 0, e => false || e.AssigneeId == input.AssigneeId);
            var status = await GetMonthStatus((DateTime)input.DateFilter);
            var pagedAndFilteredTasks = query.OrderBy(input.Sorting ?? "ClosingMonth asc").PageBy(input);
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
            }).OrderBy(x => x.DueDate);


            return new PagedResultDto<TasksGroup>(
                totalCount,
                response.ToList()
            );
        }
        #endregion
        #region|Create Eidt Details Delete|
        public async Task CreateOrEdit(CreateOrEditClosingChecklistDto input)
        {
            long checklistNextID = 1;

            if (input.Id == 0)
            {
                if (IsIterationFrequencyChanged)
                {
                    #region|Frequency changed request| 

                    var checklistCount = _closingChecklistRepository.GetAll().Max(p => (long?)p.Id) ?? 0;
                    if (input.EndOfMonth)
                    {
                        input.DueOn = 0;
                    }
                    if (input.Frequency == FrequencyDto.None)
                    {
                        string groupId = GenerateGroupId(checklistCount + checklistNextID, 0);
                        input.GroupId = groupId;
                        input.ClosingMonth = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);
                        await Create(input);
                    }
                    else if (input.NoOfMonths > 0)
                    {
                        string groupIdInitial = GenerateGroupId(checklistCount + checklistNextID, 0);
                        input.GroupId = groupIdInitial;
                        // int numberOfMonths = GetNumberOfMonthsBetweenDates(input.ClosingMonth, input.EndsOn);
                        int numberOfMonths = GetNumberOfMonthsBetweenDates(input.ClosingMonth, input.EndsOn);//10/9
                        int datDiff = CompareDates(input.ClosingMonth);
                        if (datDiff > 0)
                        {
                            input.ClosingMonth = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);
                            await Create(input);
                        }

                        input.ClosingMonth = input.ClosingMonth.AddMonths(input.NoOfMonths);
                        if (numberOfMonths > 0)
                        {
                            int numberOfRecordsToInsert = GetNumberOfTaskIterationCountForEveryXNumberOfMonth(numberOfMonths, input.NoOfMonths);
                            for (int i = 1; i <= numberOfRecordsToInsert; i++)
                            {
                                string groupId = GenerateGroupId(checklistCount + checklistNextID, i);
                                input.GroupId = groupId;
                                if (i != 1)
                                {
                                    input.ClosingMonth = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);

                                }
                                await Create(input);

                                input.ClosingMonth = GetIterationBaseDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn);
                                input.ClosingMonth = GetClosingMonthByIterationNumberForXNumberOfMonth(input.ClosingMonth, input.NoOfMonths);
                            }
                            IsIterationFrequencyChanged = false;
                            IterationStartClosingMonth = DateTime.MinValue;
                            EditedTaskClosingMonth = DateTime.MinValue;
                        }
                    }
                    else if (input.Frequency != FrequencyDto.None && input.Frequency != FrequencyDto.XNumberOfMonths)
                    {
                        string groupIdInitial = GenerateGroupId(checklistCount + checklistNextID, 0);
                        input.GroupId = groupIdInitial;
                        int numberOfMonths = GetNumberOfMonthsBetweenDates(input.ClosingMonth, input.EndsOn);
                        int frequency = GetFrequencyValue((int)input.Frequency);

                        if (frequency == 1)
                        {
                            numberOfMonths = GetNumberOfMonthsBetweenDates(input.ClosingMonth, input.EndsOn);
                        }
                        /*   int diff = CompareDates(input.ClosingMonth);

                           if (diff > 0)
                           {
                               input.ClosingMonth = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);
                               await Create(input);
                           }
                           */

                        ////////if (IsIterationFrequencyChanged)
                        ////////{

                        int diff = CompareDates(input.ClosingMonth);
                        if (diff <= 0)
                        {
                            if (frequency == 3)
                            {
                                input.ClosingMonth = input.ClosingMonth.AddMonths(3);
                            }
                            else if (frequency == 12)
                            {
                                input.ClosingMonth = input.ClosingMonth.AddMonths(12);
                            }
                        }
                        if (numberOfMonths > 0)
                        {
                            int numberOfRecordsToInsert = GetNumberOfTaskIterationCount(numberOfMonths, (int)input.Frequency);
                            for (int i = 1; i <= numberOfRecordsToInsert; i++)
                            {
                                string groupId = GenerateGroupId(checklistCount + checklistNextID, i);
                                input.GroupId = groupId;
                                input.ClosingMonth = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);
                                await Create(input);

                                input.ClosingMonth = GetIterationBaseDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn);
                                input.ClosingMonth = GetClosingMonthByIterationNumber(input.ClosingMonth, (int)input.Frequency);
                            }
                            IsIterationFrequencyChanged = false;
                            IterationStartClosingMonth = DateTime.MinValue;
                            EditedTaskClosingMonth = DateTime.MinValue;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region|Initial create request| 
                    input.ClosingMonth = input.ClosingMonth.AddDays(1);
                    input.EndsOn = input.EndsOn.AddDays(1);
                    input.EndsOn = input.EndsOn.ToUniversalTime();
                    var checklistCount = _closingChecklistRepository.GetAll().Max(p => (long?)p.Id) ?? 0;

                    if (input.EndOfMonth)
                    {
                        input.DueOn = 0;
                    }
                    if (input.Frequency == FrequencyDto.None)
                    {
                        string groupId = GenerateGroupId(checklistCount + checklistNextID, 0);
                        input.GroupId = groupId;
                        input.DueDate = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);
                        await Create(input);
                    }
                    else if (input.NoOfMonths > 0)
                    {
                        string groupIdInitial = GenerateGroupId(checklistCount + checklistNextID, 0);
                        input.GroupId = groupIdInitial;
                        // int numberOfMonths = GetNumberOfMonthsBetweenDates(input.ClosingMonth, input.EndsOn);
                        int numberOfMonths = GetNumberOfMonthsBetweenDates(input.ClosingMonth.AddMonths(1), input.EndsOn);
                        input.ClosingMonth = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);
                        await Create(input);
                        input.ClosingMonth = input.ClosingMonth.AddMonths(input.NoOfMonths);
                        if (numberOfMonths > 0)
                        {
                            int numberOfRecordsToInsert = GetNumberOfTaskIterationCountForEveryXNumberOfMonth(numberOfMonths, input.NoOfMonths);
                            for (int i = 0; i < numberOfRecordsToInsert; i++)
                            {
                                string groupId = GenerateGroupId(checklistCount + checklistNextID, i + 1);
                                input.GroupId = groupId;
                                input.ClosingMonth = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);
                                await Create(input);

                                input.ClosingMonth = GetIterationBaseDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn);
                                input.ClosingMonth = GetClosingMonthByIterationNumberForXNumberOfMonth(input.ClosingMonth, input.NoOfMonths);
                            }
                            IsIterationFrequencyChanged = false;
                            IterationStartClosingMonth = DateTime.MinValue;
                            EditedTaskClosingMonth = DateTime.MinValue;
                        }
                    }
                    else if (/*input.Frequency != FrequencyDto.None && input.Frequency != FrequencyDto.XNumberOfMonths*/false)
                    {
                        
                        int numberOfMonths = GetNumberOfMonthsBetweenDates(input.ClosingMonth, input.EndsOn);
                        int frequency = GetFrequencyValue((int)input.Frequency);
                        if (frequency == 1)
                        {
                            numberOfMonths = GetNumberOfMonthsBetweenDates(input.ClosingMonth, input.EndsOn) - 1;
                        }
                        input.DueDate = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);
                        await Create(input);
                        if (frequency == 1)
                        {
                            input.DueDate = input.ClosingMonth.AddMonths(1);
                        }
                        else
                        if (frequency == 3)
                        {
                            input.DueDate = input.ClosingMonth.AddMonths(3);
                        }
                        else if (frequency == 12)
                        {
                            input.DueDate = input.ClosingMonth.AddMonths(12);
                        }
                        if (numberOfMonths > 0)
                        {
                            int numberOfRecordsToInsert = GetNumberOfTaskIterationCount(numberOfMonths, (int)input.Frequency);
                            for (int i = 0; i < numberOfRecordsToInsert; i++)
                            {
                                string groupId = GenerateGroupId(checklistCount + checklistNextID, i + 1);
                                input.GroupId = groupId;
                                input.DueDate = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);
                                await Create(input);

                                input.DueDate = GetIterationBaseDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn);
                                input.DueDate = GetClosingMonthByIterationNumber(input.ClosingMonth, (int)input.Frequency);
                            }
                            IsIterationFrequencyChanged = false;
                            IterationStartClosingMonth = DateTime.MinValue;
                            EditedTaskClosingMonth = DateTime.MinValue;
                        }
                    }
                    //Taqi is coding here
                    else if (input.Frequency == FrequencyDto.Monthly)
                    {
                        input.GroupId = GenerateGroupId(checklistCount + checklistNextID, 0);
                         var monthDifference = await GetMonthDifference(input.ClosingMonth, input.EndsOn);
                        for (int i = 1; i <= monthDifference; i++)
                        {
                            input.DueDate = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth,
                                input.DueOn, input.EndOfMonth);
                            await Create(input);
                            var nextMonth = input.ClosingMonth.AddMonths(1);
                            input.ClosingMonth = nextMonth;

                        }
                    }
                    #endregion
                }


            }
            else
            {
                await Update(input);
            }
        }

        //taqi is coding here
        public async Task<int> GetMonthDifference(DateTime start, DateTime end)
        {
            return ((end.Year * 12 + end.Month) - (start.Year * 12 + start.Month)) + 1;
        }

        //[AbpAuthorize(AppPermissions.Pages_Tasks_Create)]
        protected virtual async Task Create(CreateOrEditClosingChecklistDto input)
        {

            var task = ObjectMapper.Map<ClosingChecklist>(input);
            if (AbpSession.TenantId != null)
            {
                task.TenantId = (int)AbpSession.TenantId;
            }
            var checklistId = await _closingChecklistRepository.InsertAndGetIdAsync(task);

            if (!String.IsNullOrWhiteSpace(input.CommentBody))
            {
                var commentDto = new CreateOrEditCommentDto()
                {
                    Body = input.CommentBody,
                    Type = CommentTypeDto.ClosingChecklist,
                    TypeId = checklistId
                };
                await _commentAppService.Create(commentDto);
            }
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
        protected virtual async Task Update(CreateOrEditClosingChecklistDto input)
        {
            var task = await _closingChecklistRepository.FirstOrDefaultAsync((int)input.Id);
            if ((Frequency)input.Frequency == task.Frequency)
            {
                if (input.EndOfMonth)
                {
                    input.DueOn = 0;
                }
                input.DueDate = GetNextIterationDateAfterDueDate(input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);
                await _closingChecklistRepository.UpdateAsync(task);
            }

            if (task.EndsOn != null)
            {
                int endsDateChange = CompareTwoDates((DateTime)task.EndsOn, input.EndsOn);
                #region|Check frequency changes|
                if (task.Frequency != (Frequency)input.Frequency || endsDateChange != 0)
                {
                    //Find task group.
                    string str = "";
                    int index = task.GroupId.IndexOf('-');
                    if (index > 0)
                    {
                        str = task.GroupId.Substring(0, index);
                    }
                    var list = _closingChecklistRepository.GetAll().Select(x => new ClosingCheckGroupDto { GroupId = x.GroupId, Id = x.Id }).ToList();
                    list = list.Where(x => x.GroupId != null).ToList();
                    var taskGroups = list.Where(x => x.GroupId.Contains(str)).ToList();
                    int iterationNumber = Convert.ToInt32(task.GroupId.Split("-").Last());
                    List<ClosingCheckGroupDto> tasksListToDiscard = GetIterationFutureTasks(taskGroups, Convert.ToInt64(str), iterationNumber);
                    foreach (var item in tasksListToDiscard)
                    {
                        await Delete(item.Id);
                    }
                    if ((int)input.Frequency != (int)Frequency.None)
                    {

                        //  input.ClosingMonth = task.ClosingMonth.AddMonths(1); // Start from the current task closingMonth
                        input.ClosingMonth = IterationStartClosingMonth;
                        // input.EndsOn = new DateTime(input.EndsOn.Year, input.EndsOn.Month, 1);
                        // EditedTaskClosingMonth = task.ClosingMonth;

                        IsIterationFrequencyChanged = true;
                        input.Id = 0;
                        Flag = false;
                        await CreateOrEdit(input);

                        //Discard all 
                        // And create tasks onward with new frequency.
                    }
                    //Test cases.
                    //If the updated frequency is none and the task have future iterations discard that.
                    //If frequency is changed other than none then discard future tasks and Re-Create from that month to the end month with updated frequency.

                    //do not disturb the current and previous iterations
                    //do not allow to change the assignee and status on edit screen
                    //Do not to allow the ClosingMonth to change.
                }
                #endregion
                else
                {
                    ObjectMapper.Map(input, task);
                    task.Status = (Status)input.Status;
                    task.Frequency = (Frequency)input.Frequency;
                    _closingChecklistRepository.Update(task);
                    if (input.comments != null)
                    {
                        foreach (var item in input.comments)
                        {
                            var commentDto = new CreateOrEditCommentDto()
                            {
                                Body = item.Body,
                                Type = CommentTypeDto.ClosingChecklist,
                                TypeId = input.Id
                            };
                            if (item.Id == 0)
                            {
                                await _commentAppService.Create(commentDto);
                            }
                            else
                            {
                                await _commentAppService.Update(commentDto);
                            }

                        }
                    }

                    if (input.CommentBody != "")
                    {
                        var commentDto = new CreateOrEditCommentDto()
                        {
                            Body = input.CommentBody,
                            Type = CommentTypeDto.ClosingChecklist,
                            TypeId = input.Id,
                        };
                        await _commentAppService.Create(commentDto);
                    }

                    if (input.AttachmentsPath != null)
                    {
                        PostAttachmentsPathDto postAttachmentsPathDto = new PostAttachmentsPathDto
                        {
                            FilePath = input.AttachmentsPath,
                            TypeId = input.Id,
                            Type = 1
                        };
                        await _attachmentAppService.PostAttachmentsPath(postAttachmentsPathDto);
                    }
                }
            }
        }
        public async Task<GetTaskForEditDto> GetTaskForEdit(long id)
        {
            var task = await _closingChecklistRepository.GetAll().Include(a => a.Assignee).Include(a => a.Category).FirstOrDefaultAsync(x => x.Id == id);
            var output = ObjectMapper.Map<GetTaskForEditDto>(task);
            output.MonthStatus = await GetMonthStatus(task.ClosingMonth);
            output.Comments = await _commentAppService.GetComments(1, id);
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
                output.Comments = await _commentAppService.GetComments(1, task.Id);
                output.Attachments = await _attachmentAppService.GetAttachmentsPath(task.Id, 1);
                output.ProfilePicture = task.Assignee.ProfilePictureId.HasValue
                    ? "data:image/jpeg;base64," + _profileAppService
                          .GetProfilePictureById((Guid)task.Assignee.ProfilePictureId).Result.ProfilePicture
                    : "";
                output.MonthStatus = await GetMonthStatus(task.ClosingMonth);
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
        public List<ClosingCheckGroupDto> GetIterationFutureTasks(List<ClosingCheckGroupDto> tasks, long id, int iterationNumber)
        {

            var list = new List<ClosingCheckGroupDto>();
            var checklists = _closingChecklistRepository.GetAll().ToList();
            foreach (var item in tasks)
            {

                int itemNumber = Convert.ToInt32(item.GroupId.Split("-").Last());
                if (itemNumber == 0)
                {
                    IterationStartClosingMonth = _closingChecklistRepository.FirstOrDefault(x => x.Id == item.Id).ClosingMonth;
                    IterationStartClosingMonth = new DateTime(IterationStartClosingMonth.Year, IterationStartClosingMonth.Month, 1);
                }
                var currentItem = checklists.FirstOrDefault(x => x.Id == item.Id);
                if (currentItem != null && CompareDates(currentItem.ClosingMonth) > 0)
                {
                    list.Add(item);
                }
            }
            return list;
        }
        #endregion
        #region|Frequency Logic for Create Task|
        public int GetNumberOfMonthsBetweenDates(DateTime closingDate, DateTime endDate)
        {
            var count = ((endDate.Year - closingDate.Year) * 12) + endDate.Month - closingDate.Month + 1;
            return count;
        }
        public int GetNumberOfTaskIterationCount(int numberOfMonths, int frequency)
        {
            int frequencyNumber = GetFrequencyValue(frequency);
            decimal iterations = (numberOfMonths / frequencyNumber);
            int iterationNumber = (int)Math.Ceiling(iterations);

            return iterationNumber;
        }
        public int GetNumberOfTaskIterationCountForEveryXNumberOfMonth(int numberOfMonths, int xNumber)
        {
            decimal iterations = (numberOfMonths / xNumber);
            int iterationNumber = (int)Math.Ceiling(iterations);
            return iterationNumber;
        }
        public DateTime GetClosingMonthByIterationNumber(DateTime closingMonth, int frequency)
        {
            var frequencyNumber = GetFrequencyValue(frequency);
            var dateTime = closingMonth.AddMonths(frequencyNumber);
            return dateTime;
        }
        public DateTime GetClosingMonthByIterationNumberForXNumberOfMonth(DateTime closingMonth, int frequency)
        {
            var dateTime = closingMonth.AddMonths(frequency);
            return dateTime;
        }
        public int GetTotalDaysByMonth(DateTime dateTime)
        {
            var totalDays = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            return totalDays;
        }
        public DateTime GetNextIterationDateAfterDueDate(bool isDaysBefore, DateTime closingMonth, int numberOfDays, bool endsOfMonth)
        {
            var nextDate = closingMonth;
            if (endsOfMonth)
            {
                nextDate = new DateTime(closingMonth.Year, closingMonth.Month, (GetTotalDaysByMonth(closingMonth)));
            }
            else if (isDaysBefore)
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
        public int GetFrequencyValue(int frequency)
        {
            var number = 1;
            switch (frequency)
            {
                case 1:
                    number = 1;
                    break;
                case 2:
                    number = 3;
                    break;
                case 3:
                    number = 12;
                    break;

                default:
                    number = 1;
                    break;
            }
            return number;
        }
        public DateTime GetIterationBaseDate(bool daysBeforeAfter, DateTime closingMonth, int numberOfDays)
        {
            DateTime baseDate = closingMonth;
            if (daysBeforeAfter)
            {
                baseDate = new DateTime(closingMonth.Year, closingMonth.Month, (GetTotalDaysByMonth(closingMonth) - numberOfDays));

            }
            else
            {

                // baseDate = new DateTime(closingMonth.Year, closingMonth.Month, (GetTotalDaysByMonth(closingMonth) - numberOfDays));
                baseDate = closingMonth.AddDays(-numberOfDays);

            }
            // var baseDate = closingMonth.AddDays(-numberOfDays);
            return baseDate;
        }
        protected virtual async Task<bool> GetMonthStatus(DateTime dateTime)
        {
            return await _managementsAppService.GetMonthStatus(dateTime);
        }
        public int CompareDates(DateTime ClosingMonth)
        {
            DateTime dateTime = DateTime.Now;
            DateTime date1 = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
            DateTime date2 = new DateTime(ClosingMonth.Year, ClosingMonth.Month, 1, 0, 0, 0);
            int result = DateTime.Compare(date2, date1);
            return result;
        }
        public int CompareTwoDates(DateTime ClosingMonth, DateTime EndsOn)
        {
            DateTime date1 = new DateTime(EndsOn.Year, EndsOn.Month, 1, 0, 0, 0);
            DateTime date2 = new DateTime(ClosingMonth.Year, ClosingMonth.Month, 1, 0, 0, 0);
            int result = DateTime.Compare(date2, date1);
            return result;
        }
        #endregion
    }
}

