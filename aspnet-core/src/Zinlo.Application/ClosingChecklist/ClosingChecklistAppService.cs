using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Zinlo.Comment;
using Zinlo.Comment.Dtos;
using Zinlo.ClosingChecklist.Dtos;
using System.Collections.Generic;
using Abp.Authorization;
using Zinlo.Authorization.Users;
using Zinlo.Attachments;
using Zinlo.Attachments.Dtos;
using Zinlo.Authorization.Users.Profile;
using NUglify.Helpers;
using Zinlo.TimeManagements;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Zinlo.Authorization.Roles;
using Zinlo.ClosingChecklist.Exporting;
using Zinlo.Dto;
using Zinlo.InstructionVersions;
using Zinlo.InstructionVersions.Dto;

namespace Zinlo.ClosingChecklist
{
    public class ClosingChecklistAppService : ZinloAppServiceBase, IClosingChecklistAppService
    {
        private readonly ClosingChecklistManager _closingChecklistManager;
        private readonly ICommentAppService _commentAppService;
        private readonly IRepository<User, long> _userRepository;
        private readonly IAttachmentAppService _attachmentAppService;
        private readonly IProfileAppService _profileAppService;
        private readonly TimeManagementManager _managementManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IInstructionAppService _instructionVersionsAppService;
        private readonly ITaskExcelExporter _taskExcelExporter;
        private readonly RoleManager _roleManager;
        private readonly IRepository<ChartofAccounts.SecondaryUserAssignee, long> _secondaryAssignee;

        public ClosingChecklistAppService(IProfileAppService profileAppService,
                                          ICommentAppService commentAppService,
                                          IRepository<User, long> userRepository,
                                          IAttachmentAppService attachmentAppService,
                                          TimeManagementManager managementManager,
                                          IUnitOfWorkManager unitOfWorkManager,
                                          IInstructionAppService instructionVersionsAppService,
                                          ClosingChecklistManager closingChecklistManager,
                                          ITaskExcelExporter taskExcelExporter,
                                          RoleManager roleManager,
                                          IRepository<ChartofAccounts.SecondaryUserAssignee, long> secondaryAssignee
                                          )
        {
            _commentAppService = commentAppService;
            _userRepository = userRepository;
            _attachmentAppService = attachmentAppService;
            _managementManager = managementManager;
            _profileAppService = profileAppService;
            _unitOfWorkManager = unitOfWorkManager;
            _instructionVersionsAppService = instructionVersionsAppService;
            _closingChecklistManager = closingChecklistManager;
            _taskExcelExporter = taskExcelExporter;
            _roleManager = roleManager;
            _secondaryAssignee = secondaryAssignee;
        }
        public async Task<PagedResultDto<TasksGroup>> GetReport(GetTaskReportInput input)
        {
            var query = GetTaskQuery(input);
            var pagedAndFilteredTasks = query.OrderBy(input.Sorting ?? "DueDate asc").PageBy(input);
            var totalCount = query.Count();
            var getUserWithPictures = (from o in query.ToList()

                                       select new GetUserWithPicture()
                                       {
                                           Id = o.AssigneeId,
                                           Name = o.Assignee.FullName,
                                           Picture = o.Assignee.ProfilePictureId.HasValue
                                               ? "data:image/jpeg;base64," + _profileAppService
                                                     .GetProfilePictureById((Guid)o.Assignee.ProfilePictureId).Result.ProfilePicture
                                               : ""
                                       }).ToList();

            getUserWithPictures = getUserWithPictures.DistinctBy(p => new { p.Id, p.Name }).ToList();

            var closingCheckList = from o in pagedAndFilteredTasks.ToList()

                                   select new ClosingCheckListForViewDto()
                                   {
                                       Id = o.Id,
                                       AssigneeId = o.AssigneeId,
                                       StatusId = (int)o.Status,
                                       TaskName = o.TaskName,
                                       Status = o.Status.ToString(),
                                       Category = o.Category.Title,
                                       DueDate = o.DueDate,
                                       IsDeleted = o.IsDeleted,
                                   };

            var result = closingCheckList.ToList();
            var response = result.GroupBy(x => x.DueDate.Date).Select(x => new TasksGroup
            {
                MonthStatus = GetMonthStatus(x.Key).Result,
                OverallMonthlyAssignee = getUserWithPictures,
                DueDate = x.Key,
                Group = x.Select(y => new ClosingCheckListForViewDto
                {
                    DueDate = y.DueDate,
                    AssigneeId = y.AssigneeId,
                    Category = y.Category,
                    StatusId = y.StatusId,
                    Id = y.Id,
                    Status = y.Status,
                    TaskName = y.TaskName,
                    IsDeleted = y.IsDeleted
                }
               )
            });



            return new PagedResultDto<TasksGroup>(
                totalCount,
                 response.ToList());

        }

        public async Task<PagedResultDto<TasksGroup>> GetAll(GetAllClosingCheckListInput input)
        {
             using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {

                var secondaryAssignee = _secondaryAssignee.GetAll().Where(x => x.SecondaryId == AbpSession.UserId && x.IsDeleted == false).Select(p => p.PrimaryId).ToList();
                var getAssignee = _closingChecklistManager.GetAll().Where(x => secondaryAssignee.Contains(x.AssigneeId)).Include(x => x.Assignee).Include(rest => rest.Category);
                
                //if (input.DateFilter < DateTime.Now.AddMonths(1))
                //    return new PagedResultDto<TasksGroup>();
                var query = _closingChecklistManager.GetAll()
                    .Where(e => e.ClosingMonth.Month == input.DateFilter.Month &&
                                e.ClosingMonth.Year == input.DateFilter.Year).Include(rest => rest.Category)
                    .Include(u => u.Assignee)
                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TaskName.Contains(input.Filter))
                    .WhereIf(input.StatusFilter != 0,
                        e => false || e.Status == (Zinlo.ClosingChecklist.Status)input.StatusFilter)
                    .WhereIf(input.CategoryFilter != 0, e => false || e.CategoryId == input.CategoryFilter)
                    .WhereIf(input.AssigneeId != 0, e => false || e.AssigneeId == input.AssigneeId)
                    .WhereIf(input.AllOrActive != true, e => (e.IsDeleted == input.AllOrActive))
                    .WhereIf(GetRoleName().Equals("User"), p => p.AssigneeId == AbpSession.UserId);

                var tempList = query.ToList();
                tempList.AddRange(getAssignee.ToList());
                var checkList = tempList.AsQueryable<ClosingChecklist>();

                var status = await GetMonthStatus((DateTime)input.DateFilter);
                var pagedAndFilteredTasks = checkList.OrderBy(input.Sorting ?? "DueDate asc").PageBy(input);
                var totalCount = checkList.Count();
                var getUserWithPictures = (from o in checkList.ToList()

                                           select new GetUserWithPicture()
                                           {
                                               Id = o.AssigneeId,
                                               Name = o.Assignee.FullName,
                                               Picture = o.Assignee.ProfilePictureId.HasValue
                                                   ? "data:image/jpeg;base64," + _profileAppService
                                                         .GetProfilePictureById((Guid)o.Assignee.ProfilePictureId).Result.ProfilePicture
                                                   : ""
                                           }).ToList();

                getUserWithPictures = getUserWithPictures.DistinctBy(p => new { p.Id, p.Name }).ToList();

                var closingCheckList = from o in pagedAndFilteredTasks.ToList()

                                       select new ClosingCheckListForViewDto()
                                       {
                                           Id = o.Id,
                                           AssigneeId = o.AssigneeId,
                                           StatusId = (int)o.Status,
                                           TaskName = o.TaskName,
                                           Status = o.Status.ToString(),
                                           Category = o.Category.Title,
                                           DueDate = o.DueDate,
                                           IsDeleted = o.IsDeleted,
                                       };

                var result = closingCheckList.ToList();
                var response = result.GroupBy(x => x.DueDate.Date).Select(x => new TasksGroup
                {
                    MonthStatus = status,
                    OverallMonthlyAssignee = getUserWithPictures,
                    DueDate = x.Key.Date,
                    Group = x.Select(y => new ClosingCheckListForViewDto
                    {
                        DueDate = y.DueDate,
                        AssigneeId = y.AssigneeId,
                        Category = y.Category,
                        StatusId = y.StatusId,
                        Id = y.Id,
                        Status = y.Status,
                        TaskName = y.TaskName,
                        IsDeleted = y.IsDeleted
                    }
                    )
                });



                return new PagedResultDto<TasksGroup>(
                    totalCount,
                    response.ToList()
                );
            }
        }

        private string GetRoleName()
        {
            if (AbpSession.UserId == null) return "User session null";
            var name =  _roleManager.GetRoleNameByUserId((long)AbpSession.UserId).Result;
            return name;

        }
        public async Task CreateOrEdit(CreateOrEditClosingChecklistDto input)
        {
            if (input.Id == 0)
            {
                if (!await _managementManager.TaskCreationValidation(input.ClosingMonth)) throw new UserFriendlyException(L("ThisMonthIsNotActive"));
                await TaskIteration(input, new DateTime(), true);
            }
            else
            {
                var editIteration = false;
                var isDueDateChanged = false;
                var currentTaskDetail = await _closingChecklistManager.GetById(input.Id);
                var checkMonthStatus = await _managementManager.IsClosed(currentTaskDetail.ClosingMonth);

                if (!checkMonthStatus)
                {
                    if (currentTaskDetail.Frequency == (Frequency)input.Frequency)
                    {
                        if (!(currentTaskDetail.EndOfMonth == input.EndOfMonth && currentTaskDetail.DueOn == input.DueOn &&
                              currentTaskDetail.DayBeforeAfter == (DaysBeforeAfter)input.DayBeforeAfter))
                        {
                            editIteration = true;
                            isDueDateChanged = true;
                        }
                    }
                    else
                    {
                        var taskList = _closingChecklistManager.GetAll().Where(p => p.GroupId == input.GroupId && p.Id != input.Id && p.ClosingMonth > input.ClosingMonth).ToList();
                        foreach (var task in taskList)
                        {
                            var checkManagementExist = await _managementManager.CheckManagementExist(task.ClosingMonth);
                            if (checkManagementExist) continue;
                            var managementDto = new TimeManagement()
                            {
                                Month = task.ClosingMonth,
                                Status = false
                            };
                            await _managementManager.CreateAsync(managementDto);
                        }
                        var openManagement = _managementManager.GetOpenManagement();
                        foreach (var item in openManagement)
                        {

                            var task = taskList.FirstOrDefault(p => p.ClosingMonth.Year == item.Month.Year && p.ClosingMonth.Month == item.Month.Month);
                            if (task != null)
                            {
                                await _closingChecklistManager.DeleteTask(task);
                                await CurrentUnitOfWork.SaveChangesAsync();
                            }
                        }

                        editIteration = true;
                    }

                    if (!editIteration && input.CategoryId != currentTaskDetail.CategoryId || input.AssigneeId != currentTaskDetail.AssigneeId || !input.TaskName.Equals(currentTaskDetail.TaskName))
                    {
                        editIteration = true;
                    }
                }
                else
                {
                    ObjectMapper.Map(input, currentTaskDetail);
                }
                if (editIteration)
                {
                    await UpdateTaskProperties(input, isDueDateChanged);
                }
                if (!string.IsNullOrWhiteSpace(input.Instruction))
                {
                    var comparisonOfInstruction = false;
                    if (currentTaskDetail.VersionId != null)
                    {
                        comparisonOfInstruction = await _instructionVersionsAppService.Comparison((long)currentTaskDetail.VersionId, input.Instruction);
                    }

                    if (comparisonOfInstruction == false)
                    {
                        currentTaskDetail.VersionId = await CreateInstructions(input.Instruction);
                        await _closingChecklistManager.UpdateVersionIds(currentTaskDetail.VersionId, currentTaskDetail.GroupId);
                    }
                }
                else
                {
                    if (currentTaskDetail.VersionId != null)
                    {
                        currentTaskDetail.VersionId = null;
                        await _closingChecklistManager.UpdateVersionIds(currentTaskDetail.VersionId, currentTaskDetail.GroupId);
                    }
                }
                await CreateComment(input.CommentBody, input.Id);
                CurrentUnitOfWork.SaveChanges();
            }
        }
        protected virtual async Task UpdateTaskProperties(CreateOrEditClosingChecklistDto input, bool isDueDatetChanged)
        {
            var taskList = _closingChecklistManager.GetAll().Where(p => p.GroupId == input.GroupId).ToList();
            foreach (var task in taskList)
            {
                var checkManagementExist = await _managementManager.CheckManagementExist(task.ClosingMonth);
                if (!checkManagementExist)
                {
                    var managementDto = new TimeManagement()
                    {
                        Month = task.ClosingMonth,
                        Status = false
                    };
                    await _managementManager.CreateAsync(managementDto);
                }
            }
            var openManagement = _managementManager.GetOpenManagement();
            foreach (var item in openManagement)
            {

                var task = taskList.FirstOrDefault(p => p.ClosingMonth.Year == item.Month.Year && p.ClosingMonth.Month == item.Month.Month);
                if (task != null)
                {
                    task.CategoryId = input.CategoryId;
                    task.AssigneeId = input.AssigneeId;
                    task.TaskName = input.TaskName;
                    task.DueOn = input.DueOn;
                    task.TaskUpdatedTime = DateTime.UtcNow;
                    if (isDueDatetChanged)
                    {
                        task.DueDate = _closingChecklistManager.GetDueDate((DaysBeforeAfter)input.DayBeforeAfter, task.ClosingMonth, input.DueOn, input.EndOfMonth);
                    }
                    await Update(task);
                }
            }

        }
        protected virtual async Task ChangeTaskFrequency(CreateOrEditClosingChecklistDto input)
        {
            var taskList = _closingChecklistManager.GetAll().Where(p => p.GroupId == input.GroupId).ToList();
            foreach (var task in taskList)
            {
                var checkManagementExist = await _managementManager.CheckManagementExist(task.ClosingMonth);
                if (!checkManagementExist)
                {
                    var managementDto = new TimeManagement()
                    {
                        Month = task.ClosingMonth,
                        Status = false
                    };
                    await _managementManager.CreateAsync(managementDto);
                }
            }
            var managementDetail = _managementManager.GetOpenManagement();
            foreach (var item in managementDetail)
            {
                var task = taskList.FirstOrDefault(p => p.ClosingMonth.Year == item.Month.Year && p.ClosingMonth.Month == item.Month.Month);
                if (task != null) await _closingChecklistManager.DeleteAsync(task.Id);
                CurrentUnitOfWork.SaveChanges();
            }

            await TaskIteration(input, new DateTime(), true);
        }

        protected virtual async Task Create(CreateOrEditClosingChecklistDto input)
        {
            var monthClosed = await _managementManager.IsClosed(input.ClosingMonth);
            if (monthClosed)
            {
                return;
            }
            long? instructionId = input.VersionId;
            var task = ObjectMapper.Map<ClosingChecklist>(input);
            if (AbpSession.TenantId != null)
            {
                task.TenantId = (int)AbpSession.TenantId;
            }

            if (!String.IsNullOrWhiteSpace(input.Instruction))
            {

                instructionId = await CreateInstructions(input.Instruction);
            }

            task.VersionId = instructionId;
            var checklistId = await _closingChecklistManager.InsertAndGetIdAsync(task);

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
            CurrentUnitOfWork.SaveChanges();
        }

        protected virtual async Task<long> CreateInstructions(string input)
        {
            var instruction = new CreateOrEditInstructionVersion
            {
                Body = input
            };
            return await _instructionVersionsAppService.CreateOrEdit(instruction);
        }
        protected virtual async Task Update(ClosingChecklist entity)
        {
            await _closingChecklistManager.UpdateAsync(entity);
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
            var task = await _closingChecklistManager.GetDetailById(id);

            var output = ObjectMapper.Map<GetTaskForEditDto>(task);
            output.MonthStatus = await GetMonthStatus(task.ClosingMonth);
            output.InstructionBody = task.Version?.Body ?? string.Empty;
            output.Comments = await _commentAppService.GetComments((int)CommentTypeDto.ClosingChecklist, id);
            output.Attachments = await _attachmentAppService.GetAttachmentsPath(task.Id, 1);
            output.ProfilePicture = task.Assignee.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)task.Assignee.ProfilePictureId).Result.ProfilePicture : "";
            return output;
        }
        public async Task<DetailsClosingCheckListDto> GetDetails(long id)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var task = await _closingChecklistManager.GetDetailById(id);
                var output = ObjectMapper.Map<DetailsClosingCheckListDto>(task);
                if (task == null) return output;
                output.Id = task.Id;
                output.AssigneeName = task.Assignee.Name;
                output.TaskStatus = task.Status.ToString();
                output.Status = (StatusDto)task.Status;
                output.CategoryName = task.Category.Title;
                output.CategoryId = task.CategoryId;
                output.InstructionBody = task.Version?.Body ?? string.Empty;
                output.Comments = await _commentAppService.GetComments((int)CommentTypeDto.ClosingChecklist, task.Id);
                output.Attachments = await _attachmentAppService.GetAttachmentsPath(task.Id, 1);
                output.ProfilePicture = task.Assignee.ProfilePictureId.HasValue
                    ? "data:image/jpeg;base64," + _profileAppService
                          .GetProfilePictureById((Guid)task.Assignee.ProfilePictureId).Result.ProfilePicture
                    : "";
                output.MonthStatus = await GetMonthStatus(task.ClosingMonth);
                output.DueDate = task.DueDate.Date;
                output.IsDeleted = task.IsDeleted;

                return output;
            }
        }
        public async Task Delete(long id)
        {
            await _closingChecklistManager.DeleteAsync(id);
        }
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
        public async Task ChangeAssignee(ChangeAssigneeDto input)
        {
            await _closingChecklistManager.ChangeAssignee(input.TaskId, input.AssigneeId);
        }
        public async Task ChangeStatus(ChangeStatusDto input)
        {
            await _closingChecklistManager.ChangeStatus(input.TaskId, input.StatusId);
        }

        protected virtual async Task<bool> GetMonthStatus(DateTime dateTime)
        {
            return await _managementManager.GetMonthStatus(dateTime);
        }

        private bool CheckTaskExist(DateTime closingMonth, Guid? groupId)
        {
            return _closingChecklistManager.CheckTaskExist(closingMonth, groupId);
        }
        public async Task RestoreTask(long id)
        {
            await _closingChecklistManager.RestoreTask(id);

        }

        public async Task RevertInstruction(long id, long instructionId)
        {
            await _closingChecklistManager.RevertInstruction(id, instructionId);
        }

        public List<NameValueDto<string>> GetCurrentMonthDays(DateTime dateTime)
        {
            return _closingChecklistManager.GetCurrentMonthDays(dateTime);
        }

        public async Task TaskIteration(CreateOrEditClosingChecklistDto input, DateTime openingMonth, bool singleIteration)
        {
            Guid? oldGroupId = null;
            var forEdit = false;
            if (input.Id == 0)
            {
                input.ClosingMonth = input.ClosingMonth.AddDays(1);
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
                        input.DueDate = _closingChecklistManager.GetDueDate((DaysBeforeAfter)input.DayBeforeAfter, input.ClosingMonth, input.DueOn, input.EndOfMonth);

                        var taskExist = CheckTaskExist(input.ClosingMonth, oldGroupId);
                        if (!taskExist)
                        {
                            if (forEdit) input.Id = 0;
                            await Create(input);
                        }
                        break;
                    }
                case FrequencyDto.Monthly:
                    {
                        int monthDifference = 1;
                        if (!singleIteration)
                        {
                            monthDifference = _closingChecklistManager.GetMonthDifference(input.ClosingMonth, openingMonth);
                        }

                        for (int i = 1; i <= monthDifference; i++)
                        {
                            input.DueDate = _closingChecklistManager.GetDueDate((DaysBeforeAfter)input.DayBeforeAfter, input.ClosingMonth,
                                input.DueOn, input.EndOfMonth);
                            var taskExist = CheckTaskExist(input.ClosingMonth, oldGroupId);
                            if (!taskExist)
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
                        int monthDifference = 1;
                        if (!singleIteration)
                        {
                            monthDifference = _closingChecklistManager.GetMonthDifference(input.ClosingMonth, openingMonth);
                        }

                        for (int i = 1; i <= monthDifference; i += 3)
                        {
                            input.DueDate = _closingChecklistManager.GetDueDate((DaysBeforeAfter)input.DayBeforeAfter, input.ClosingMonth,
                                input.DueOn, input.EndOfMonth);
                            var taskExist = CheckTaskExist(input.ClosingMonth, oldGroupId);
                            if (!taskExist)
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
                        int yearDifference = 0;
                        if (!singleIteration)
                        {
                            yearDifference = _closingChecklistManager.GetDifferenceInYears(input.ClosingMonth, openingMonth);
                        }
                        for (int i = 0; i <= yearDifference; i++)
                        {
                            input.DueDate = _closingChecklistManager.GetDueDate((DaysBeforeAfter)input.DayBeforeAfter, input.ClosingMonth,
                                input.DueOn, input.EndOfMonth);
                            var taskExist = CheckTaskExist(input.ClosingMonth, oldGroupId);
                            if (!taskExist)
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
                        int monthDifference = 1;
                        if (!singleIteration)
                        {
                            monthDifference = _closingChecklistManager.GetMonthDifference(input.ClosingMonth, openingMonth);
                        }
                        for (int i = 1; i <= monthDifference; i += input.NoOfMonths)
                        {
                            input.DueDate = _closingChecklistManager.GetDueDate((DaysBeforeAfter)input.DayBeforeAfter, input.ClosingMonth,
                                input.DueOn, input.EndOfMonth);
                            var taskExist = CheckTaskExist(input.ClosingMonth, oldGroupId);
                            if (input.Id == 0 || !taskExist)
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

        public async Task<List<CreateOrEditClosingChecklistDto>> GetTaskTimeDuration(DateTime input)
        {
            var lastYear = input.AddYears(-1).AddMonths(-1);
            var query = _closingChecklistManager.GetAll()
                .Where(p => p.ClosingMonth >= lastYear && p.ClosingMonth <= input).ToList();
            var groupBy = query.GroupBy(i => i.GroupId);
            var taskList = new List<ClosingChecklist>();
            foreach (var group in groupBy)
            {
                var getLatestUpdatedTask = group.FirstOrDefault(t => t.TaskUpdatedTime.HasValue);
                if (getLatestUpdatedTask == null)
                {
                    taskList.Add(group.OrderByDescending(t => t.ClosingMonth).FirstOrDefault());
                }
                else
                {
                    taskList.Add(group.OrderByDescending(t => t.TaskUpdatedTime).FirstOrDefault());
                }
            }

            return ObjectMapper.Map<List<CreateOrEditClosingChecklistDto>>(taskList);
        }

        public async Task<FileDto> GetTaskToExcel(GetTaskToExcelInput input)
        {
            var query = GetTaskQuery(input);
            var tasks = query
                .OrderBy(input.Sorting ?? "DueDate asc")
                .ToList();
            var taskListDtos = ObjectMapper.Map<List<TaskListDto>>(tasks);
            await FillStatusName(taskListDtos);

            return _taskExcelExporter.ExportToFile(taskListDtos);

        }

        protected virtual async Task FillStatusName(IEnumerable<TaskListDto> input)
        {
            foreach (var taskListDto in input)
            {
                if (taskListDto.Status.Equals("1"))
                {
                    taskListDto.Status = "Not started";
                }
                else if (taskListDto.Status.Equals("2"))
                {
                    taskListDto.Status = "In process";
                }
                else if (taskListDto.Status.Equals("3"))
                {
                    taskListDto.Status = "On hold";
                }
                else if (taskListDto.Status.Equals("4"))
                {
                    taskListDto.Status = "Completed";
                }
            }

        }
        private IQueryable<ClosingChecklist> GetTaskQuery(IGetTaskInput input)
        {
            return _closingChecklistManager.GetAll().Include(rest => rest.Category)
                .Include(u => u.Assignee)
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TaskName.Contains(input.Filter))
                .WhereIf(input.StatusFilter != 0,
                    e => false || e.Status == (Zinlo.ClosingChecklist.Status)input.StatusFilter)
                .WhereIf(input.CategoryFilter != 0, e => false || e.CategoryId == input.CategoryFilter)
                .WhereIf(input.AssigneeId != 0, e => false || e.AssigneeId == input.AssigneeId)
                .WhereIf(input.StartDate != null && input.EndDate != null, e => e.DueDate >= input.StartDate && e.DueDate <= input.EndDate);
        }
    }
}

