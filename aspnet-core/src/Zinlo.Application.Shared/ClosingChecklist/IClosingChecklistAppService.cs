using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using Zinlo.ClosingChecklist.Dtos;
using Zinlo.Tasks.Dtos;

namespace Zinlo.ClosingChecklist
{
    public interface IClosingChecklistAppService : IApplicationService
    {
      
        Task CreateOrEdit(CreateOrEditClosingChecklistDto input);

      ///  Task <GetClosingCheckListTaskDto> GetClosingCheckListTask();
        Task<PagedResultDto<GetClosingCheckListTaskDto>> GetAll();


    }
}
