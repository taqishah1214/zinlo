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
namespace Zinlo.ClosingChecklist
{
    public class ClosingChecklistAppService : ZinloAppServiceBase, IClosingChecklistAppService
    {
        private readonly IRepository<ClosingChecklist,long> _closingChecklistRepository;
        private readonly ICommentAppService _commentAppService;
        private readonly IRepository<User,long> _userRepository;
        private readonly IRepository<Comment.Comment, int> _commentRepository;
        public ClosingChecklistAppService
            (    
        IRepository<ClosingChecklist,long> closingChecklistRepository, 
        ICommentAppService commentAppService, 
        IRepository<User,long> userRepository,
        IRepository<Comment.Comment, int> commentRepository
            )
        {
            _closingChecklistRepository = closingChecklistRepository;
            _commentAppService = commentAppService;
            _userRepository = userRepository;
            _commentRepository = commentRepository;
        }


        public enum Status
        {
            Open = 1,
            Complete = 2,
            Inprogress = 3
        }

        public async Task<PagedResultDto<GetClosingCheckListTaskDto>> GetAll()
        {
            var query = _closingChecklistRepository.GetAll().Include(rest => rest.Category).Include(u=>u.AssigneeName);

            var closingCheckList = from o in query
                                   select new GetClosingCheckListTaskDto()
                                   {
                                       ClosingCheckListForViewDto = new ClosingCheckListForViewDto
                                       {     Id = o.Id,
                                            AssigneeId = o.AssigneeName.Id,
                                             StatusId = (int) o.Status,
                                          AssigniName = o.AssigneeName.FullName,
                                           TaskName = o.TaskName,
                                           Status =  o.Status.ToString(),
                                           Category =o.Category.Title,
                                           
                                          
                                       }
                             };

            var totalCount = await closingCheckList.CountAsync();

            return new PagedResultDto<GetClosingCheckListTaskDto>(
                totalCount,
                await closingCheckList.ToListAsync()
            );
        }


        public async System.Threading.Tasks.Task CreateOrEdit(CreateOrEditClosingChecklistDto input)
        {

            await Create(input);

            /*if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }*/
        }

        

        //[AbpAuthorize(AppPermissions.Pages_Tasks_Create)]
        protected virtual  async System.Threading.Tasks.Task Create(CreateOrEditClosingChecklistDto input)
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
            var category = await _closingChecklistRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, category);
        }

        public async Task<List<NameValueDto<string>>> UserAutoFill(string searchTerm)
        {
            var filteredAssets = _userRepository.GetAll()
                .WhereIf(!string.IsNullOrWhiteSpace(searchTerm),
                    e => true || e.FullName.ToLower().Contains(searchTerm.ToLower()));


            var query = (from o in filteredAssets

                         select new NameValueDto<string>()
                         {
                             Name = o.FullName,
                             Value = o.Id.ToString()
                         });

            var assets = await query.ToListAsync();
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
            var task = _closingChecklistRepository.GetAll().Include(u=>u.AssigneeName).Where(x => x.Id == id).FirstOrDefault();
            DetailsClosingCheckListDto detailsClosingCheckListDto = new DetailsClosingCheckListDto();
            detailsClosingCheckListDto.Id = task.Id;
            detailsClosingCheckListDto.TaskName = task.TaskName;
            detailsClosingCheckListDto.Instruction = task.Instruction;
            detailsClosingCheckListDto.ClosingMonth = task.ClosingMonth;
            detailsClosingCheckListDto.EndsOn = task.EndsOn;
            detailsClosingCheckListDto.DayBeforeAfter = task.DayBeforeAfter;
            detailsClosingCheckListDto.AssigneeName = task.AssigneeName.Name;
            detailsClosingCheckListDto.comments = await GetTaskCommentById(task.Id);
            return detailsClosingCheckListDto;

        }
        protected  async Task<List<CommentDto>> GetTaskCommentById(long id) {
            List<CommentDto> commentList = new List<CommentDto>();
            var taskComments = await  _commentRepository.GetAll().Where(x => x.Type == CommentType.ClosingChecklist && x.TypeId == id).ToListAsync();
            if(taskComments.Count > 0)
            {
                foreach (var comment in taskComments)
                {
                    var commentObj = ObjectMapper.Map<CommentDto>(comment);
                    commentList.Add(commentObj);
                }
                return commentList;
            }
            else
            {
                return new List<CommentDto>();
            }

            
        }

        public async Task ChangeAssignee(ChangeAssigneeDto changeAssigneeDto)
        {
            var task = await _closingChecklistRepository.FirstOrDefaultAsync(changeAssigneeDto.TaskId);
            if(task != null)
            {
                task.AssigneeNameId = changeAssigneeDto.AssigneeId;
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

      
    }
}
