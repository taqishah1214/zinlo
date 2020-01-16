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

namespace Zinlo.ClosingChecklist
{
    public class ClosingChecklistAppService : ZinloAppServiceBase, IClosingChecklistAppService
    {
        private readonly IRepository<ClosingChecklist,long> _closingChecklistRepository;
        private readonly ICommentAppService _commentAppService;

        public ClosingChecklistAppService(IRepository<ClosingChecklist,long> closingChecklistRepository, ICommentAppService commentAppService)
        {
            _closingChecklistRepository = closingChecklistRepository;
            _commentAppService = commentAppService;
        }


        public async Task<PagedResultDto<GetClosingCheckListTaskDto>> GetAll()
        {
            var query = _closingChecklistRepository.GetAll().Include(rest => rest.Category).Include(u=>u.AssigneeName);

            var closingCheckList = from o in query
                                   select new GetClosingCheckListTaskDto()
                                   {
                                       ClosingCheckListForViewDto = new ClosingCheckListForViewDto
                                       {
                                          AssigniName = o.AssigneeName.FullName,
                                           TaskName = o.TaskName,
                                         //  Status =  (int)o.Status,
                                           Category =o.Category.Title
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
       

       

     
    }
}
