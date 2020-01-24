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
namespace Zinlo.ClosingChecklist
{
    public class ClosingChecklistAppService : ZinloAppServiceBase, IClosingChecklistAppService
    {
        private readonly IRepository<ClosingChecklist,long> _closingChecklistRepository;
        private readonly ICommentAppService _commentAppService;
        private readonly IRepository<User,long> _userRepository;
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

        public async Task<PagedResultDto<GetClosingCheckListTaskDto>> GetAll()
        {
            var query = _closingChecklistRepository.GetAll().Include(rest => rest.Category).Include(u=>u.Assignee);

            var closingCheckList = from o in query
                                   select new GetClosingCheckListTaskDto()
                                   {
                                       ClosingCheckListForViewDto = new ClosingCheckListForViewDto
                                       { Id = o.Id,
                                           AssigneeId = o.AssigneeId,
                                           StatusId = (int)o.Status,
                                           AssigniName = o.Assignee.FullName,      //_userRepository.FirstOrDefaultAsync(o.AssigneeId).Result.FullName,
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
            getTaskForEditDto.Status = task.Status.ToString();

            return getTaskForEditDto;


        }
    }
}
