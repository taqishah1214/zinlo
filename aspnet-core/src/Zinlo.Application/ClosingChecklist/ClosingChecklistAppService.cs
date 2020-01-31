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
using Zinlo.ClosingChecklist;
using Zinlo.Tasks.Dtos;
using Zinlo.Comment;
using Zinlo.Comment.Dtos;
using Zinlo.ClosingChecklist.Dtos;
using System.Collections.Generic;
using Zinlo.Authorization.Users;
using Zinlo.ClosingChecklist;
using Zinlo.Attachments;
using Zinlo.Attachments.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Zinlo.ClosingChecklist
{
    public class ClosingChecklistAppService : ZinloAppServiceBase, IClosingChecklistAppService
    {
        private readonly IRepository<ClosingChecklist,long> _closingChecklistRepository;
        private readonly ICommentAppService _commentAppService;
        private readonly IRepository<User,long> _userRepository;
        private readonly IAttachmentAppService _attachmentAppService;


        public ClosingChecklistAppService(IRepository<ClosingChecklist,long> closingChecklistRepository, ICommentAppService commentAppService, IRepository<User,long> userRepository)
        {
            _closingChecklistRepository = closingChecklistRepository;
            _commentAppService = commentAppService;
            _userRepository = userRepository;
        }


        public enum Status
        {
            Open = 1,
            Complete = 2,
            Inprogress = 3
        }

        public async Task<PagedResultDto<GetClosingCheckListTaskDto>> GetAll(GetAllClosingCheckListInput input)
        {
            var query = _closingChecklistRepository.GetAll().Include(rest => rest.Category).Include(u => u.Assignee)
                                    .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Category.Title.Contains(input.Filter) || e.Category.Title.Contains(input.Filter));
            var totalCount = await query.CountAsync();
            var pagedAndFilteredTasks = query.OrderBy(input.Sorting ?? "id asc").PageBy(input);
            var closingCheckList = from o in pagedAndFilteredTasks
                                   select new GetClosingCheckListTaskDto()
                                   {
                                       ClosingCheckListForViewDto = new ClosingCheckListForViewDto
                                       { Id = o.Id,
                                           AssigneeId = o.AssigneeId,
                                           StatusId = (int)o.Status,
                                           AssigniName = o.Assignee.FullName,      
                                           TaskName = o.TaskName,
                                           Status =  o.Status.ToString(),
                                           Category =o.Category.Title,
                                          CreationTime = o.CreationTime
                                       }
                             };



            return new PagedResultDto<GetClosingCheckListTaskDto>(
                totalCount,
                await closingCheckList.ToListAsync()
            );
        }


        public async System.Threading.Tasks.Task CreateOrEdit(CreateOrEditClosingChecklistDto input)
        {

          

            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }

        

        //[AbpAuthorize(AppPermissions.Pages_Tasks_Create)]
        protected virtual  async System.Threading.Tasks.Task Create([FromForm]CreateOrEditClosingChecklistDto input)
        {
            var task = ObjectMapper.Map<ClosingChecklist>(input);

            if (AbpSession.TenantId != null)
            {
                task.TenantId = (int)AbpSession.TenantId;
            }
            try
            {
                var checklistId = await _closingChecklistRepository.InsertAndGetIdAsync(task);
          
            if(input.CommentBody != "")
            {
                var commentDto = new CreateOrEditCommentDto() {
                    Body = input.CommentBody,
                    Type = CommentTypeDto.ClosingChecklist,
                    TypeId = checklistId
                };
                await _commentAppService.Create(commentDto);
            }
             

            }
            catch (Exception e)
            {

                throw;
            }
        }

        //[AbpAuthorize(AppPermissions.Pages_Tasks_Edit)]
        protected virtual async System.Threading.Tasks.Task Update(CreateOrEditClosingChecklistDto input)
        {
            try
            {
                var task = await _closingChecklistRepository.FirstOrDefaultAsync((int)input.Id);
                //   var data = ObjectMapper.Map(input, task);
                task.TaskName = input.TaskName;
                task.CategoryId = input.CategoryId;
                task.DayBeforeAfter = input.DayBeforeAfter;
                task.EndOfMonth = input.EndOfMonth;
                task.Instruction = input.Instruction;
                task.ClosingMonth = input.ClosingMonth;
                task.Status = (Zinlo.ClosingChecklist.Status) input.Status;
                task.Frequency = (Zinlo.ClosingChecklist.Frequency)input.Frequency;
                task.AssigneeId = input.AssigneeId;
                task.DueOn = input.DueOn;
              var result =   _closingChecklistRepository.Update(task);
                var r = result;
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
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error");

            }
           
           

        }

        public async Task<List<NameValueDto<string>>> UserAutoFill(string searchTerm)
        {
            List<User> list = await _userRepository.GetAll().ToListAsync();
            if(!String.IsNullOrEmpty(searchTerm))
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
            var assets =  query;
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

        public async Task<DetailsClosingCheckListDto> getDetails(long id)
        {
            var task = _closingChecklistRepository.GetAll().Include(u=>u.Assignee).Include(c=>c.Category).Where(x => x.Id == id).FirstOrDefault();
            DetailsClosingCheckListDto detailsClosingCheckListDto = new DetailsClosingCheckListDto();
            detailsClosingCheckListDto.Id = task.Id;
            detailsClosingCheckListDto.TaskName = task.TaskName;
            detailsClosingCheckListDto.Instruction = task.Instruction;
            detailsClosingCheckListDto.ClosingMonth = task.ClosingMonth;
            detailsClosingCheckListDto.EndsOn = task.EndsOn;
            detailsClosingCheckListDto.DayBeforeAfter = task.DayBeforeAfter;
            detailsClosingCheckListDto.AssigneeName = task.Assignee.Name;
            detailsClosingCheckListDto.DueOn = task.DueOn;
            detailsClosingCheckListDto.NoOfMonths = task.NoOfMonths;
            detailsClosingCheckListDto.Status = (StatusDto)task.Status;
            detailsClosingCheckListDto.TaskStatus = task.Status.ToString();
            detailsClosingCheckListDto.Instruction = task.Instruction;
            detailsClosingCheckListDto.CategoryName = task.Category.Title;
            detailsClosingCheckListDto.comments = await _commentAppService.GetComments(1, task.Id);
            return detailsClosingCheckListDto;

        }

        public async Task ChangeAssignee(ChangeAssigneeDto changeAssigneeDto)
        {
            var task = await _closingChecklistRepository.FirstOrDefaultAsync(changeAssigneeDto.TaskId);
            if(task != null)
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
                task.Status =  (Zinlo.ClosingChecklist.Status)changeStatusDto.StatusId;
                _closingChecklistRepository.Update(task);
            }
        }

        public async Task<GetTaskForEditDto> GetTaskForEdit(long id)
        {
            var task = await _closingChecklistRepository.GetAll().Where(x => x.Id == id).Include(a => a.Assignee).Include(a => a.Category).FirstOrDefaultAsync();

            GetTaskForEditDto getTaskForEditDto = new GetTaskForEditDto();
            getTaskForEditDto.Id = task.Id;
            getTaskForEditDto.AssigniName = task.Assignee.FullName;
            getTaskForEditDto.Category = task.Category.Title;
            getTaskForEditDto.ClosingMonth = task.ClosingMonth;
            getTaskForEditDto.DayBeforeAfter = task.DayBeforeAfter;
            getTaskForEditDto.DueOn = task.DueOn;
            getTaskForEditDto.EndsOn = task.EndsOn;
            getTaskForEditDto.Frequency = task.Frequency.ToString();
            getTaskForEditDto.Instruction = task.Instruction;
            getTaskForEditDto.comments = await _commentAppService.GetComments(1, id);
            getTaskForEditDto.NoOfMonths = task.NoOfMonths;
            getTaskForEditDto.TaskName = task.TaskName;
            getTaskForEditDto.AssigneeId = task.AssigneeId;
            getTaskForEditDto.CategoryId = task.CategoryId;
            getTaskForEditDto.FrequencyId = (int)task.Frequency;
            getTaskForEditDto.Status = task.Status.ToString();
            return getTaskForEditDto;
        }

        public async Task Delete(long id)
        {
            await _closingChecklistRepository.DeleteAsync(id);
        }

    }
}
