using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Zinlo.TimeManagements.Dtos;

namespace Zinlo.TimeManagements
{
    public interface ITimeManagementsAppService : IApplicationService 
    {
        Task<PagedResultDto<GetTimeManagementForViewDto>> GetAll(GetAllTimeManagementsInput input);

		Task<GetTimeManagementForEditOutput> GetTimeManagementForEdit(EntityDto<long> input);

		Task CreateOrEdit(CreateOrEditTimeManagementDto input);

		Task Delete(EntityDto<long> input);
		Task ChangeStatus(long id);

		
    }
}