using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.Dto;

namespace Zinlo.ChartsofAccount
{
   public interface IChartsofAccountAppService : IApplicationService
    {
        Task CreateOrEdit(CreateOrEditChartsofAccountDto input);
        Task Delete(long id);
        Task<GetAccountForEditDto> GetAccountForEdit(long id);
        Task<PagedResultDto<ChartsofAccoutsForViewDto>> GetAll(GetAllChartsofAccountInput input);
        Task ChangeAccountsAssignee(long accountId, long assigneeId);
        Task ChangeStatus(long accountId, long selectedStatusId);
        Task AddandUpdateBalance(double balance, long id);
        Task <int> CheckReconcilled(long id);
        Task<FileDto> GetChartsofAccountToExcel(long id);
        Task<bool> CheckAccountForTrialBalance(string accountName, string accountNumber, string trialBalance);
        Task<double> GetTrialBalanceofAccount(long id);
        bool CheckAccountNoExist(string AccountNumber);
        
    }
}
