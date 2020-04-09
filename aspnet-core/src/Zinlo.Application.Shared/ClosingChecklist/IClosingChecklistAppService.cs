using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zinlo.ClosingChecklist.Dtos;
using Zinlo.Tasks.Dtos;

namespace Zinlo.ClosingChecklist
{
    public interface IClosingChecklistAppService : IApplicationService
    {

        Task<PagedResultDto<TasksGroup>> GetAll(GetAllClosingCheckListInput input);
        Task CreateOrEdit(CreateOrEditClosingChecklistDto input);
        Task<DetailsClosingCheckListDto> GetDetails(long id);
        Task ChangeAssignee(ChangeAssigneeDto changeAssigneeDto);
        Task ChangeStatus(ChangeStatusDto changeStatusDto);
        Task <GetTaskForEditDto>GetTaskForEdit(long id);
        Task Delete(long id);
        Task<List<GetUserWithPicture>> GetUserWithPicture(string searchTerm,long? id);
        Task RestoreTask(long id);

    }
}
