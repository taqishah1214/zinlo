using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Tasks.Dtos;
using Abp.Extensions;
using Abp.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Zinlo.Authorization;

namespace Zinlo.Tasks
{
    public class TasksAppService : ZinloAppServiceBase, ITaskAppService
    {
        private readonly IRepository<Task> _taskRepository;

        public TasksAppService(IRepository<Task> taskRepository)
        {
            _taskRepository = taskRepository;
        }
        public async Task<PagedResultDto<GetTaskForViewDto>> GetAll(GetAllTasksInput input)
        {
            var filteredTasks = _taskRepository.GetAll();
            var pagedAndFilteredTasks = filteredTasks
                .OrderBy(input.Sorting ?? "id asc")
                .PageBy(input);

            var tasks = from t in pagedAndFilteredTasks
                        select new GetTaskForViewDto()
                        {
                            Task = new TaskDto
                            {
                                TaskName = t.TaskName,
                                CategoryId = t.CategoryId,
                                ClosingMonth = t.ClosingMonth,
                                FilePath = t.FilePath,
                                UserId = t.UserId,
                                TenantId = t.TenantId
                            }
                        };

            var totalCount = await filteredTasks.CountAsync();

            return new PagedResultDto<GetTaskForViewDto>(
                totalCount,
                await tasks.ToListAsync()
            );
        }
        public async System.Threading.Tasks.Task CreateOrEdit(CreateOrEditTaskDto input)
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

        [AbpAuthorize(AppPermissions.Pages_Tasks_Create)]
        protected virtual  async System.Threading.Tasks.Task Create(CreateOrEditTaskDto input)
        {
            var task = ObjectMapper.Map<Task>(input);


            if (AbpSession.TenantId != null)
            {
                task.TenantId = (int)AbpSession.TenantId;
            }


            await _taskRepository.InsertAsync(task);
        }

        [AbpAuthorize(AppPermissions.Pages_Tasks_Edit)]
        protected virtual async System.Threading.Tasks.Task Update(CreateOrEditTaskDto input)
        {
            var category = await _taskRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, category);
        }
        public System.Threading.Tasks.Task Delete(EntityDto input)
        {
            throw new NotImplementedException();
        }

       

        public async Task<GetTaskForEditOutput> GetTaskForEdit(EntityDto input)
        {
            var task = await _taskRepository.FirstOrDefaultAsync(input.Id);
            var output = new GetTaskForEditOutput { Task = ObjectMapper.Map<CreateOrEditTaskDto>(task) };
            return output;
        }

        public async Task<GetTaskForViewDto> GetTaskForView(int id)
        {
            var task = await _taskRepository.GetAsync(id);
            var output = new GetTaskForViewDto { Task = ObjectMapper.Map<TaskDto>(task) };
            return output;
        }
        
    }
}
