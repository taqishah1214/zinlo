using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using Zinlo.Tasks.Dtos;

namespace Zinlo.ClosingChecklist
{
    public interface IClosingChecklistAppService : IApplicationService
    {
        Task<PagedResultDto<GetTaskForViewDto>> GetAll(GetAllTasksInput input);

        Task<GetTaskForViewDto> GetTaskForView(int id);

        Task<GetTaskForEditOutput> GetTaskForEdit(EntityDto input);

        Task CreateOrEdit(CreateOrEditClosingChecklistDto input);

        Task Delete(EntityDto input);
    }
}
