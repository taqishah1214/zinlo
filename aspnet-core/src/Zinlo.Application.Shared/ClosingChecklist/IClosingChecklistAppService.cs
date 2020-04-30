using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zinlo.ClosingChecklist.Dtos;
using Zinlo.Dto;

namespace Zinlo.ClosingChecklist
{
    public interface IClosingChecklistAppService : IApplicationService
    {
        Task<PagedResultDto<TasksGroup>> GetReport(GetTaskReportInput input);  
        Task<PagedResultDto<TasksGroup>> GetAll(GetAllClosingCheckListInput input);
        Task CreateOrEdit(CreateOrEditClosingChecklistDto input);
        Task<DetailsClosingCheckListDto> GetDetails(long id);
        Task ChangeAssignee(ChangeAssigneeDto changeAssigneeDto);
        Task ChangeStatus(ChangeStatusDto changeStatusDto);
        Task <GetTaskForEditDto>GetTaskForEdit(long id);
        Task Delete(long id);
        Task<List<GetUserWithPicture>> GetUserWithPicture(string searchTerm,long? id);
        Task RestoreTask(long id);
        Task RevertInstruction(long id, long instructionId);
        List<NameValueDto<string>> GetCurrentMonthDays(DateTime dateTime);
        Task TaskIteration(CreateOrEditClosingChecklistDto input, DateTime openingMonth, bool singleIteration);
        Task<List<CreateOrEditClosingChecklistDto>> GetTaskTimeDuration(DateTime input);
        Task<FileDto> GetTaskToExcel(GetTaskToExcelInput input);

    }
}
