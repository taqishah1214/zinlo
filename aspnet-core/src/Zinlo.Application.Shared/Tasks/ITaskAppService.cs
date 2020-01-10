using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Dto;
using Zinlo.Tasks.Dtos;

namespace Zinlo.Tasks
{
 public   interface ITaskAppService : IApplicationService
    {
        Task<PagedResultDto<GetTaskForViewDto>> GetAll(GetAllTasksInput input);

        Task<GetTaskForViewDto> GetTaskForView(int id);

        Task<GetTaskForEditOutput> GetTaskForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditTaskDto input);

        Task Delete(EntityDto input);

       // Task<FileDto> GetCategoriesToExcel(GetAllCategoriesForExcelInput input);
    }
}
