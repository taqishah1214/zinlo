﻿using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Threading.Tasks;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Zinlo.Authorization;
using Zinlo.ClosingChecklist;
using Zinlo.Tasks.Dtos;
using Zinlo.Comment;
using Zinlo.Comment.Dtos;
using Zinlo.ClosingChecklist.Dtos;
using System.Collections.Generic;
using Zinlo.Authorization.Users;
using Zinlo.Attachments;
using Zinlo.Attachments.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Zinlo.Authorization.Users.Profile;

namespace Zinlo.ClosingChecklist
{
    public class ClosingChecklistAppService : ZinloAppServiceBase, IClosingChecklistAppService
    {
        private readonly IRepository<ClosingChecklist, long> _closingChecklistRepository;
        private readonly ICommentAppService _commentAppService;
        private readonly IRepository<User, long> _userRepository;
        private readonly IAttachmentAppService _attachmentAppService;
        private readonly UserManager _userManager;
        private readonly IProfileAppService _profileAppService;



        public ClosingChecklistAppService(IProfileAppService profileAppService, IRepository<ClosingChecklist, long> closingChecklistRepository, UserManager userManager, ICommentAppService commentAppService, IRepository<User, long> userRepository, IAttachmentAppService attachmentAppService)
        {
            _closingChecklistRepository = closingChecklistRepository;
            _commentAppService = commentAppService;
            _userRepository = userRepository;
            _attachmentAppService = attachmentAppService;
            _userManager = userManager;
            _profileAppService = profileAppService;
        }


        public enum Status
        {
            Open = 1,
            Complete = 2,
            Inprogress = 3
        }
        public async Task<PagedResultDto<TasksGroup>> GetAll(GetAllClosingCheckListInput input)
     {      
                int index = input.MonthFilter.IndexOf('/');
                int month = Convert.ToInt32( input.MonthFilter.Substring(0, index));
                int year =  Convert.ToInt32( input.MonthFilter.Substring(index+1, 4));
            var query = _closingChecklistRepository.GetAll().Include(rest => rest.Category).Include(u => u.Assignee)
                                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.TaskName.Contains(input.Filter))
                                    .WhereIf(input.StatusFilter != 0, e => false || e.Status == (Zinlo.ClosingChecklist.Status)input.StatusFilter)
                                    .WhereIf(input.CategoryFilter != 0, e => false || e.CategoryId == input.CategoryFilter)
                                        .WhereIf(month != 100 && year != 2000, e => false || e.CreationTime.Month == month && e.CreationTime.Year == year)
                                    .WhereIf(input.DateFilter != null && input.DateFilter.Value.Date.Year != 2000, e => false || e.CreationTime.Date == input.DateFilter.Value.Date)
                                    .WhereIf(month == 100 && year == 2000 && input.DateFilter != null && input.DateFilter.Value.Date.Year == 2000, e => false || e.CreationTime.Month == DateTime.Today.Month && e.CreationTime.Year == DateTime.Today.Year);
            var pagedAndFilteredTasks = query.OrderBy(input.Sorting ?? "id asc").PageBy(input);
            var totalCount = query.Count();
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
                                       CreationTime = o.CreationTime,
                                       ProfilePicture = o.Assignee.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)o.Assignee.ProfilePictureId).Result.ProfilePicture : ""

        };
            var result =  closingCheckList.ToList();
            var response = result.GroupBy(x => x.CreationTime.Date).Select(x => new TasksGroup
            {
                CreationTime = x.Key,
                group = x.Select(y => new ClosingCheckListForViewDto
                {
                    CreationTime = y.CreationTime,
                    AssigneeId = y.AssigneeId,
                    Category = y.Category,
                    StatusId = y.StatusId,
                    Id = y.Id,
                    AssigniName = y.AssigniName,
                    Status = y.Status,
                    TaskName = y.TaskName,
                    ProfilePicture = y.ProfilePicture
                }
                )
            }).OrderBy(x=>x.CreationTime);

             
            return new PagedResultDto<TasksGroup>(
                totalCount,
                response.ToList()
            );
        }


        public async System.Threading.Tasks.Task CreateOrEdit(CreateOrEditClosingChecklistDto input)
        {
            if (input.Id == 0 || input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        //[AbpAuthorize(AppPermissions.Pages_Tasks_Create)]
        protected virtual async System.Threading.Tasks.Task Create(CreateOrEditClosingChecklistDto input)
        {
            var task = ObjectMapper.Map<ClosingChecklist>(input);

            if (AbpSession.TenantId != null)
            {
                task.TenantId = (int)AbpSession.TenantId;
            }
            try
            {
                var checklistId = await _closingChecklistRepository.InsertAndGetIdAsync(task);

                if (input.CommentBody != "")
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
                    PostAttachmentsPathDto postAttachmentsPathDto = new PostAttachmentsPathDto();
                    postAttachmentsPathDto.FilePath = input.AttachmentsPath;
                    postAttachmentsPathDto.TypeId = checklistId;
                    postAttachmentsPathDto.Type = 1;
                    await _attachmentAppService.PostAttachmentsPath(postAttachmentsPathDto);
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

    
        protected virtual async Task Update(CreateOrEditClosingChecklistDto input)
        {
                var task = await _closingChecklistRepository.FirstOrDefaultAsync((int)input.Id);
                 var data = ObjectMapper.Map(input, task);
                task.Status = (Zinlo.ClosingChecklist.Status)input.Status;
                task.Frequency = (Zinlo.ClosingChecklist.Frequency)input.Frequency;
                var result = _closingChecklistRepository.Update(task);
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
                        if (item.Id == null)
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
                PostAttachmentsPathDto postAttachmentsPathDto = new PostAttachmentsPathDto();
                postAttachmentsPathDto.FilePath = input.AttachmentsPath;
                postAttachmentsPathDto.TypeId = input.Id;
                postAttachmentsPathDto.Type = 1;
                await _attachmentAppService.PostAttachmentsPath(postAttachmentsPathDto);
            }
        }

        public async Task<List<NameValueDto<string>>> UserAutoFill(string searchTerm)
        {
            List<User> list = await _userRepository.GetAll().ToListAsync();
            if (!String.IsNullOrEmpty(searchTerm))
            {
                list = list.Where(x => x.FullName.ToLower().Contains(searchTerm.Trim().ToLower())).ToList();
            }
            else
            {
                list = new List<User>();
            }
            var query = (from o in list
                         select new NameValueDto<string>()
                         {
                             Name = o.FullName,
                             Value = o.Id.ToString()
                         }).ToList();
            var assets = query;
            return assets;
        }
        public async Task<List<GetUserWithPicture>> GetUserWithPicture(string searchTerm,long? id )
        {
            List<User> list = await _userRepository.GetAll().ToListAsync();
            if (!String.IsNullOrEmpty(searchTerm))
            {
                list = list.Where(x => x.FullName.ToLower().Contains(searchTerm.Trim().ToLower())).ToList();
            }
            else
            {
                list = list.Where(x => x.Id == id).ToList(); ;
            }
            var query = (from o in list
                select new GetUserWithPicture()
                {
                    Name = o.FullName,
                    Id = o.Id,
                    Picture =  o.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)o.ProfilePictureId).Result.ProfilePicture : ""
        }).ToList();
            var assets = query;
            return assets;
        }
        public async Task<List<NameValueDto<string>>> getUsersDropdown()
        {
            var filteredUsers = _userRepository.GetAll();

            var query = (from o in filteredUsers

                         select new NameValueDto<string>()
                         {
                             Value = o.Id.ToString(),
                             Name = o.FullName
                         });

            var users = await query.ToListAsync();
            return users;
        }

        public async Task<DetailsClosingCheckListDto> GetDetails(long id)
        {
            var task = _closingChecklistRepository.GetAll().Include(u => u.Assignee).Include(c => c.Category).Where(x => x.Id == id).FirstOrDefault();
            var output = ObjectMapper.Map<DetailsClosingCheckListDto>(task);
            output.AssigneeName = task.Assignee.Name;
            output.TaskStatus = task.Status.ToString();
            output.Status = (StatusDto)task.Status;
            output.CategoryName = task.Category.Title;
            output.comments = await _commentAppService.GetComments(1, task.Id);
            output.Attachments = await _attachmentAppService.GetAttachmentsPath(task.Id, 1);
            output.ProfilePicture = task.Assignee.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)task.Assignee.ProfilePictureId).Result.ProfilePicture : "";
            return output;
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
                task.Status = (Zinlo.ClosingChecklist.Status)changeStatusDto.StatusId;
                _closingChecklistRepository.Update(task);
            }
        }

        public async Task<GetTaskForEditDto> GetTaskForEdit(long id)
        {
            var task = await _closingChecklistRepository.GetAll().Where(x => x.Id == id).Include(a => a.Assignee).Include(a => a.Category).FirstOrDefaultAsync();

            var output = ObjectMapper.Map<GetTaskForEditDto>(task);
            output.AssigniName = task.Assignee.FullName;
            output.Category = task.Category.Title;
            output.Frequency = task.Frequency.ToString();
            output.FrequencyId = (int)task.Frequency;
            output.Status = task.Status.ToString();
            output.StatusId = (int)task.Status;
            output.comments = await _commentAppService.GetComments(1, id);
            output.Attachments =  await _attachmentAppService.GetAttachmentsPath(task.Id, 1);
            output.ProfilePicture = task.Assignee.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)task.Assignee.ProfilePictureId).Result.ProfilePicture : "";
            return output;
        }

        public async Task Delete(long id)
        {
            await _closingChecklistRepository.DeleteAsync(id);
        }

        public async Task<List<NameValueDto<string>>> GetCurrentMonthDays()
        {
            List<NameValueDto<string>> list = new List<NameValueDto<string>>();
            DateTime now = DateTime.Now;
            var startDate = new DateTime(now.Year, now.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                NameValueDto<string> nameValueDto = new NameValueDto<string>();
                nameValueDto.Value = date.Day.ToString();
                nameValueDto.Name = date.Day.ToString();
                list.Add(nameValueDto);
            }
            return list;
        }
    }
}
