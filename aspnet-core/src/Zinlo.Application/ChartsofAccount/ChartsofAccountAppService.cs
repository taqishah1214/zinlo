using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Threading.Tasks;
using Zinlo.ChartsofAccount.Dtos;
using Abp.Application.Services.Dto;
using Zinlo.Authorization.Users.Profile;
using Zinlo.ClosingChecklist.Dtos;
using NUglify.Helpers;
using Zinlo.ChartofAccounts;
using Zinlo.Dto;
using Zinlo.Reconciliation;
using Zinlo.Reconciliation.Dtos;
using AccountType = Zinlo.ChartofAccounts.AccountType;
using ReconciliationType = Zinlo.ChartofAccounts.ReconciliationType;

namespace Zinlo.ChartsofAccount
{
    public class ChartsofAccountAppService : ZinloAppServiceBase, IChartsofAccountAppService
    {
        private readonly IRepository<ChartofAccounts.ChartofAccounts, long> _chartsofAccountRepository;
        private readonly IProfileAppService _profileAppService;
        private readonly IChartsOfAccountsListExcelExporter _chartsOfAccountsListExcelExporter;
        private readonly IRepository<Amortization, long> _amortizationRepository;
        private readonly IRepository<Itemization, long> _itemizationRepository;
        private readonly IChartsOfAccountsTrialBalanceExcelExporter _chartsOfAccountsTrialBalanceExcelExporter;

        public ChartsofAccountAppService(IRepository<Itemization, long> itemizationRepository,
                                         IRepository<Amortization, long> amortizationRepository,
                                         IRepository<ChartofAccounts.ChartofAccounts, long> chartsofAccountRepository,
                                         IProfileAppService profileAppService,
                                         IChartsOfAccountsListExcelExporter chartsOfAccountsListExcelExporter,
                                         IChartsOfAccountsTrialBalanceExcelExporter chartsOfAccountsTrialBalanceExcelExporter)
        {
            _amortizationRepository = amortizationRepository;
            _itemizationRepository = itemizationRepository;
            _chartsofAccountRepository = chartsofAccountRepository;
            _profileAppService = profileAppService;
            _chartsOfAccountsListExcelExporter = chartsOfAccountsListExcelExporter;
            _chartsOfAccountsTrialBalanceExcelExporter = chartsOfAccountsTrialBalanceExcelExporter;
        }

        public async Task<PagedResultDto<ChartsofAccoutsForViewDto>> GetAll(GetAllChartsofAccountInput input)
        {
            var query = _chartsofAccountRepository.GetAll().Where(e => e.ClosingMonth.Month == DateTime.Now.Month && e.ClosingMonth.Year == DateTime.Now.Year).Include(p => p.AccountSubType).Include(p => p.Assignee)
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.AccountName.Contains(input.Filter))
                 .WhereIf(input.AccountType != 0, e => (e.AccountType == (AccountType)input.AccountType))
                 .WhereIf(input.AssigneeId != 0, e => (e.AssigneeId == input.AssigneeId));

            var getUserWithPictures = (from o in query.ToList()
                                       select new GetUserWithPicture()
                                       {
                                           Id = o.AssigneeId,
                                           Name = o.Assignee.FullName,
                                           Picture = o.Assignee.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)o.Assignee.ProfilePictureId).Result.ProfilePicture : ""
                                       }).ToList();

            getUserWithPictures = getUserWithPictures.DistinctBy(p => new { p.Id, p.Name }).ToList();
            var pagedAndFilteredAccounts = query.OrderBy(input.Sorting ?? "id asc").PageBy(input);
            var totalCount = query.Count();

            var accountsList = from o in pagedAndFilteredAccounts.ToList()

                               select new ChartsofAccoutsForViewDto()
                               {
                                   Id = o.Id,
                                   AccountName = o.AccountName,
                                   AccountNumber = o.AccountNumber,
                                   AccountTypeId = (int)o.AccountType,
                                   AccountSubTypeId = o.AccountSubType.Id,
                                   AccountSubType = o.AccountSubType != null ? o.AccountSubType.Title : "",
                                   ReconciliationTypeId = o.ReconciliationType != 0 ? (int)o.ReconciliationType : 0,
                                   AssigneeName = o.Assignee != null ? o.Assignee.FullName : "",
                                   ProfilePicture = o.Assignee != null && o.Assignee.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)o.Assignee.ProfilePictureId).Result.ProfilePicture : "",
                                   AssigneeId = o.Assignee.Id,
                                   StatusId = (int)o.Status,
                                   Balance = o.Balance,
                                   OverallMonthlyAssignee = getUserWithPictures,
                                   Lock = o.Lock
                               };

            return new PagedResultDto<ChartsofAccoutsForViewDto>(
               totalCount,
               accountsList.ToList()
           );

        }
        public async Task<long> CreateOrEdit(CreateOrEditChartsofAccountDto input)
        {
            if (input.Id == 0)
            {
                return await Create(input);
            }
            return await Update(input);

        }
        protected virtual async Task<long> Update(CreateOrEditChartsofAccountDto input)
        {
            var account = await _chartsofAccountRepository.FirstOrDefaultAsync(input.Id);

            if ((int)account.ReconciliationType != (int)input.ReconciliationType)
            {
                DateTime ClosingMonth = account.ClosingMonth;
                await _chartsofAccountRepository.DeleteAsync(account);
                var previousAccountId = input.Id;
                if ((int)account.ReconciliationType == 1)
                {
                    var query = await _itemizationRepository.FirstOrDefaultAsync(x => x.ChartsofAccountId == previousAccountId && x.ClosingMonth.Month == DateTime.Now.Month);
                    if (query != null)
                    {
                        _itemizationRepository.Delete(query);
                    }

                }
                else
                {
                    var query = await _amortizationRepository.FirstOrDefaultAsync(x => x.ChartsofAccountId == previousAccountId && x.ClosingMonth.Month == DateTime.Now.Month);
                    if (query != null)
                    {
                        _amortizationRepository.Delete(query);
                    }
                }

                input.Id = 0;
                input.ClosingMonth = ClosingMonth;
                return await Create(input);
            }

            var updatedAccount = ObjectMapper.Map(input, account);
            updatedAccount.ReconciliationType = (ReconciliationType)input.ReconciliationType;
            updatedAccount.AccountType = (AccountType)input.AccountType;
            var accountId = await _chartsofAccountRepository.InsertOrUpdateAndGetIdAsync(updatedAccount);
            return accountId;


        }

        protected virtual async Task<long> Create(CreateOrEditChartsofAccountDto input)
        {
            var account = ObjectMapper.Map<ChartofAccounts.ChartofAccounts>(input);
            account.Status = (Status)2;
            if (AbpSession.TenantId != null)
            {
                account.TenantId = (int)AbpSession.TenantId;
            }
            var accountId = _chartsofAccountRepository.InsertAndGetId(account);
            return accountId;
        }
        public async Task Delete(long id)
        {
            await _chartsofAccountRepository.DeleteAsync(id);
        }
        public async Task<GetAccountForEditDto> GetAccountForEdit(long id)
        {
            var account = await _chartsofAccountRepository.GetAll().Where(x => x.Id == id).Include(a => a.Assignee).Include(a => a.AccountSubType).FirstOrDefaultAsync();
            var mappedAccount = ObjectMapper.Map<GetAccountForEditDto>(account);
            mappedAccount.AccountSubType = account.AccountSubType.Title;
            mappedAccount.AssigniId = account.Assignee.Id;
            mappedAccount.AccountType = (int)account.AccountType;
            mappedAccount.ReconcillationType = (int)account.ReconciliationType;
            mappedAccount.AccountSubTypeId = account.AccountSubType.Id;
            mappedAccount.AssigniName = account.Assignee.FullName;
            mappedAccount.AccountName = account.AccountName;
            mappedAccount.AccountNumber = account.AccountNumber;
            mappedAccount.ReconciledId = (int)account.Reconciled;
            return mappedAccount;
        }

        public async Task ChangeAccountsAssignee(long accountId, long assigneeId)
        {
            var account = await _chartsofAccountRepository.FirstOrDefaultAsync(accountId);
            if (account != null)
            {
                account.AssigneeId = assigneeId;
                _chartsofAccountRepository.Update(account);
            }
        }

        public async Task ChangeStatus(long accountId, long selectedStatusId)
        {
            var account = await _chartsofAccountRepository.FirstOrDefaultAsync(accountId);
            if (account != null)
            {
                account.Status = (Status)selectedStatusId;
                _chartsofAccountRepository.Update(account);
            }
        }

        public async Task AddandUpdateBalance(double balance, long id)
        {
            var account = await _chartsofAccountRepository.FirstOrDefaultAsync(id);
            if (account != null)
            {
                account.Balance = balance;
                await _chartsofAccountRepository.UpdateAsync(account);
            }
        }

        public async Task<int> CheckReconcilled(long id)
        {
            int result = 0;
            var account = await _chartsofAccountRepository.FirstOrDefaultAsync(id);
            if (account != null)
            {
                result = (int)account.Reconciled;
            }
            return result;
        }

        public async Task<FileDto> GetChartsofAccountToExcel(long id)
        {
            var accounts = _chartsofAccountRepository.GetAll().Include(x => x.Assignee).Include(p => p.AccountSubType);
            var listToExport = new List<ChartsOfAccountsExcellExporterDto>();
            foreach (var item in accounts)
            {
                ChartsOfAccountsExcellExporterDto chartsOfAccountsExcelExporterDto =
                    new ChartsOfAccountsExcellExporterDto
                    {
                        AccountName = item.AccountName,
                        AccountNumber = item.AccountNumber,
                        AccountType = GetAccounttypeById((int)item.AccountType),
                        AssignedUser = item.Assignee.EmailAddress,
                        AccountSubType = item.AccountSubType.Title,
                        ReconciliationType = GetReconcilationType((int)item.ReconciliationType),
                        ReconciliationAs = GetReconcilationAsValue((int)item.Reconciled)
                    };


                listToExport.Add(chartsOfAccountsExcelExporterDto);
            }
            return _chartsOfAccountsListExcelExporter.ExportToFile(listToExport);
        }
        public async Task<FileDto> LoadChartsofAccountTrialBalanceToExcel()
        {
            var accounts = _chartsofAccountRepository.GetAll().Include(x => x.Assignee);
            var listToExport = accounts.Select(item => new ChartsOfAccountsTrialBalanceExcellImportDto
            {
                AccountName = item.AccountName,
                AccountNumber = item.AccountNumber,
                Balance = item.TrialBalance.ToString()
            }).ToList();
            return _chartsOfAccountsTrialBalanceExcelExporter.ExportToExcell(listToExport);
        }
        public string GetAccounttypeById(int id)
        {
            string type;
            switch (id)
            {
                case 1:
                    type = "Equity";
                    break;
                case 2:
                    type = "Assets";
                    break;
                case 3:
                    type = "Liability";
                    break;

                default:
                    type = "Equity";
                    break;
            }
            return type;
        }

        public async Task<bool> CheckAccountForTrialBalance(ChartsOfAccountsTrialBalanceExcellImportDto input)
        {

            var result = await _chartsofAccountRepository.FirstOrDefaultAsync(x => x.AccountName.ToLower().Equals(input.AccountName.Trim().ToLower())
                                                                              && x.AccountNumber.ToLower().Equals(input.AccountNumber.Trim().ToLower()));


            if (result != null)
            {
                result.TrialBalance = Convert.ToDecimal(input.Balance);
                result.VersionId = input.VersionId;
                await _chartsofAccountRepository.UpdateAsync(result);

                return true;
            }

            return false;
        }
        public async Task<double> GetTrialBalanceofAccount(long id)
        {
            double result = 0;
            var account = await _chartsofAccountRepository.FirstOrDefaultAsync(id);
            if (account != null)
            {
                result = (double)account.TrialBalance;
            }
            return result;
        }

        public async Task<bool> CheckAccountNumber(string accountNumber)
        {
            if (accountNumber != null)
            {
                var isExist = await _chartsofAccountRepository.FirstOrDefaultAsync(e => e.AccountNumber.Trim().ToLower() == accountNumber.Trim().ToLower());
                if (isExist != null) return true;
            }
            return false;
        }

        public bool CheckAccounts()
        {
            var result = _chartsofAccountRepository.GetAll().Count();
            return result > 0;
        }
        public string GetReconcilationType(int value)
        {
            return value == 1 ? "Itemized" : "Amortization";
        }
        public async Task ShiftAmortizedItems(double previousAccountId, double newAccountId, DateTime closingMonth)
        {
            var amortizedItemList = _amortizationRepository.GetAll().Where(e => e.ChartsofAccount.Id == previousAccountId && e.ClosingMonth.Month == DateTime.Now.Month).Include(e => e.ChartsofAccount);
            var itemList = (from o in amortizedItemList.ToList()

                            select new CreateOrEditAmortizationDto()
                            {
                                Id = 0,
                                InoviceNo = o.InoviceNo,
                                JournalEntryNo = o.JournalEntryNo,
                                StartDate = o.StartDate,
                                EndDate = o.EndDate,
                                AccomulateAmount = o.AccomulateAmount,
                                Amount = o.Amount,
                                Description = o.Description,
                                ChartsofAccountId = (long)newAccountId,
                                ClosingMonth = closingMonth,
                                Criteria = (Reconciliation.Dtos.Criteria)o.Criteria,
                                CreatorUserId = o.CreatorUserId,
                            }).ToList();

            foreach (var item in itemList)
            {
                var newItem = ObjectMapper.Map<Amortization>(item);
                await _amortizationRepository.InsertAsync(newItem);
            }

        }



        public async Task ShiftItemizedItem(long previousAccountId, long newAccountId, DateTime closingMonth)
        {
            var itemizedItemList = _itemizationRepository.GetAll().Where(e => e.ChartsofAccount.Id == previousAccountId && e.ClosingMonth.Month == DateTime.Now.Month).Include(e => e.ChartsofAccount);
            var itemList = (from o in itemizedItemList.ToList()

                            select new CreateOrEditItemizationDto()
                            {
                                Id = 0,
                                InoviceNo = o.InoviceNo,
                                JournalEntryNo = o.JournalEntryNo,
                                Date = o.Date,
                                Amount = o.Amount,
                                Description = o.Description,
                                ChartsofAccountId = newAccountId,
                                ClosingMonth = closingMonth,
                                CreatorUserId = o.CreatorUserId,
                            }).ToList();

            foreach (var newitem in itemList.Select(item => ObjectMapper.Map<Itemization>(item)))
            {
                await _itemizationRepository.InsertAsync(newitem);
            }

        }

        public async Task ShiftChartsOfAccountToSpecficMonth(DateTime closingMonth)
        {
            var accountsExistCheck = await _chartsofAccountRepository.FirstOrDefaultAsync(e => e.ClosingMonth.Month == closingMonth.Month);
            if (accountsExistCheck == null)
            {
                var currentMonthAccounts = _chartsofAccountRepository.GetAll().Where(e => e.ClosingMonth.Month == DateTime.Now.Month).Include(a => a.Assignee).Include(a => a.AccountSubType);
                var itemList = (from o in currentMonthAccounts.ToList()

                                select new CreateOrEditChartsofAccountDto()
                                {
                                    Id = (int)o.Id,
                                    CreatorUserId = o.CreatorUserId,
                                    AccountName = o.AccountName,
                                    AccountNumber = o.AccountNumber,
                                    AccountType = (Dtos.AccountType)o.AccountType,
                                    ReconciliationType = (Dtos.ReconciliationType)o.ReconciliationType,
                                    AccountSubTypeId = o.AccountSubType.Id,
                                    Reconciled = (Dtos.Reconciled)o.Reconciled,
                                    Balance = 0,
                                    ClosingMonth = closingMonth,
                                    AssigneeId = o.Assignee.Id
                                }).ToList();

                foreach (var item in itemList)
                {
                    var previousAccountId = item.Id;
                    item.Id = 0;
                    var newAccountId = await CreateOrEdit(item);
                    if ((int)item.ReconciliationType == 1)
                    {
                        await ShiftItemizedItem(previousAccountId, newAccountId, closingMonth);
                    }
                    else
                    {
                        await ShiftAmortizedItems(previousAccountId, newAccountId, closingMonth);
                    }
                }
            }
        }
        public string GetReconcilationAsValue(int value)
        {
            switch (value)
            {
                case 1:
                    return "NetAmount";
                case 2:
                    return "BeginningAmount";
                case 3:
                    return "AccruedAmount";
                default:
                    return string.Empty;
            }
        }

    }
}



