using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zinlo.ClosingChecklist.Dtos;
using Zinlo.Tasks.Dtos;

namespace Zinlo.ClosingChecklist
{
    public interface IClosingChecklistAppService : IApplicationService
    {
      
        Task CreateOrEdit(CreateOrEditClosingChecklistDto input);

      ///  Task <GetClosingCheckListTaskDto> GetClosingCheckListTask();
        Task<PagedResultDto<TasksGroup>> GetAll(GetAllClosingCheckListInput input);
        Task<List<NameValueDto<string>>> UserAutoFill(string searchTerm);
        Task<DetailsClosingCheckListDto> getDetails(long id);
        Task ChangeAssignee(ChangeAssigneeDto changeAssigneeDto);
        Task ChangeStatus(ChangeStatusDto changeStatusDto);

        Task <GetTaskForEditDto>GetTaskForEdit(long id);

        Task Delete(long id);

    }
}
