﻿using Abp.Application.Services;
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
        Task<long> CreateOrEdit(CreateOrEditChartsofAccountDto input);
        Task Delete(long id);
        Task<GetAccountForEditDto> GetAccountForEdit(long id);
        Task<PagedResultDto<ChartsofAccoutsForViewDto>> GetAll(GetAllChartsofAccountInput input);
        Task ChangeAccountsAssignee(long accountId, long assigneeId);
        Task ChangeStatus(long accountId, int selectedStatusId, DateTime SelectedMonth);
        Task AddandUpdateBalance(double balance, long id, DateTime month);
        Task<int> CheckReconcilled(long id);
        Task<FileDto> GetChartsofAccountToExcel(long id);
        bool AddTrialBalanceInAccount(ChartsOfAccountsTrialBalanceExcellImportDto input);
        Task<double> GetTrialBalanceofAccount(long id, DateTime month);
        Task<bool> CheckAccountNumber(string accountNumber);
        Task<FileDto> LoadChartsofAccountTrialBalanceToExcel();
        //Task ShiftChartsOfAccountToSpecficMonth(DateTime closingMonth);
        bool CheckAccounts();
        Task RestoreAccount(long id);
        Task CheckAsReconciliedMonthly(long id, DateTime month);
        Task<LinkedAccountInfo> GetLinkAccountDetails(long accountId, DateTime month);
        Task<double> ValidateAccuredAmount(long id, double amount);

    }
}