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
using Abp.Domain.Uow;
using Zinlo.TimeManagements;
using Zinlo.TimeManagements.Dto;

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
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<TimeManagement, long> _timeManagementRepository;
        private readonly IRepository<AccountBalance, long> _accountBalanceRepository;


        public ChartsofAccountAppService(IRepository<Itemization, long> itemizationRepository,
                                         IRepository<Amortization, long> amortizationRepository,
                                         IRepository<ChartofAccounts.ChartofAccounts, long> chartsofAccountRepository,
                                         IProfileAppService profileAppService,
                                         IChartsOfAccountsListExcelExporter chartsOfAccountsListExcelExporter,
                                         IChartsOfAccountsTrialBalanceExcelExporter chartsOfAccountsTrialBalanceExcelExporter,
                                         IUnitOfWorkManager unitOfWorkManager,
                                         IRepository<TimeManagement, long> timeManagementRepository,
                                         IRepository<AccountBalance, long> accountBalanceRepository)
        {
            _amortizationRepository = amortizationRepository;
            _itemizationRepository = itemizationRepository;
            _chartsofAccountRepository = chartsofAccountRepository;
            _profileAppService = profileAppService;
            _chartsOfAccountsListExcelExporter = chartsOfAccountsListExcelExporter;
            _chartsOfAccountsTrialBalanceExcelExporter = chartsOfAccountsTrialBalanceExcelExporter;
            _unitOfWorkManager = unitOfWorkManager;
            _timeManagementRepository = timeManagementRepository;
            _accountBalanceRepository = accountBalanceRepository;
        }

        public async Task<PagedResultDto<ChartsofAccoutsForViewDto>> GetAll(GetAllChartsofAccountInput input)
        {
            
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {

                var query = _chartsofAccountRepository.GetAll().Where(x => x.IsDeleted == input.AllOrActive).Include(p => p.AccountSubType).Include(p => p.Assignee)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => e.AccountName.ToLower().Contains(input.Filter.ToLower()) || e.AccountNumber.ToLower().Contains(input.Filter.ToLower()))
                        .WhereIf(input.AccountType != 0, e => (e.AccountType == (AccountType)input.AccountType))
                        .WhereIf(input.AssigneeId != 0, e => (e.AssigneeId == input.AssigneeId))
                        .WhereIf(input.BeginingAmountCheck, e => (e.Reconciled == ChartofAccounts.Reconciled.BeginningAmount));

                var MonthStatus = await GetMonthStatus(input.SelectedMonth);
                var getUserWithPictures = (from o in query.ToList()
                                           select new GetUserWithPicture()
                                           {
                                               Id = o.AssigneeId,
                                               Name = o.Assignee.FullName,
                                               Picture = o.Assignee.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)o.Assignee.ProfilePictureId).Result.ProfilePicture : ""
                                           }).ToList();

                var changeItems = query.Where(e => e.IsChange == true);
                List<ChartofAccounts.ChartofAccounts> chartsofAccounts = new List<ChartofAccounts.ChartofAccounts>();
                var query1 =  query.Where(e => !e.IsChange);
                var result = query1.ToList();

                var distinctAccounts = changeItems.DistinctBy(p => new {p.AccountNumber }).ToList();


                // selcted month =dec 2018
                //Creation time feb2019 changetime dec2019
                //Creation time jan2020 changetime march2020
                //Creation time april2020 changetime null
                var changeItemGorup = changeItems.ToList().GroupBy(p => p.AccountNumber);
                foreach (var item in changeItems)
                {
                    if (item.ChangeTime != null)
                    {
                        if ((item.ChangeTime.Value.Month >= input.SelectedMonth.Month && item.ChangeTime.Value.Year >= input.SelectedMonth.Year) &&
                            (item.CreationTime.Month <= input.SelectedMonth.Month && item.CreationTime.Year <= input.SelectedMonth.Year))
                        {
                            chartsofAccounts.Add(item);
                        }

                    }
                    else if (item.CreationTime.Month <= input.SelectedMonth.Month && item.CreationTime.Year <= input.SelectedMonth.Year)
                    {
                        chartsofAccounts.Add(item);
                    }
                }

                var chartOfAccountList = chartsofAccounts.Select(x => x.AccountNumber).ToList();
                var accountInBothList = distinctAccounts.Where(y => !chartOfAccountList.Contains(y.AccountNumber)).ToList();

                if (accountInBothList.Count >0)
                {
                    foreach (var item in accountInBothList)
                    {
                        var missingAccount = query.Where(y => y.AccountNumber == item.AccountNumber).ToList();
                        var account = missingAccount.OrderBy(a => a.CreationTime).FirstOrDefault();
                        chartsofAccounts.Add(account);

                    }
                }
            

                result.AddRange(chartsofAccounts);


                getUserWithPictures = getUserWithPictures.DistinctBy(p => new { p.Id, p.Name }).ToList();
                var pagedAndFilteredAccounts = result;


                var totalCount = result.Count();

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
                                       AssigneeId = o.Assignee.Id,
                                       StatusId = GetStatusofSelectedMonth(o.Id, input.SelectedMonth),
                                       Balance = GetTheAccountBalanceofSelectedMonth(o.Id, input.SelectedMonth),
                                       OverallMonthlyAssignee = getUserWithPictures,
                                       IsDeleted = o.IsDeleted,
                                       MonthStatus = MonthStatus,
                                       AccountReconciliationCheck = CheckAccountReconciledofSelectedMonth(o.Id,input.SelectedMonth),
                                       AccountBalanceId = getTheAccountBalanceId(o.Id, input.SelectedMonth),
                                       TrialBalance = GetTheTrialBalanceofSelectedMonth(o.Id, input.SelectedMonth)
                                   };

                return new PagedResultDto<ChartsofAccoutsForViewDto>(
                   totalCount,
                   accountsList.ToList()
               );

            }

        }

        protected virtual long getTheAccountBalanceId(long accountId, DateTime SelectedMonth)
        {
            long result = 0;
            var accountInformation = _accountBalanceRepository.FirstOrDefault(p => p.Month.Month == SelectedMonth.Month && p.Month.Year == SelectedMonth.Year && p.AccountId == accountId);
            result = accountInformation == null ? 0 : accountInformation.Id;
            return result;
        }



        protected virtual bool CheckAccountReconciledofSelectedMonth(long accountId, DateTime SelectedMonth)
        {
            bool result = false;
            var accountInformation = _accountBalanceRepository.FirstOrDefault(p => p.Month.Month == SelectedMonth.Month && p.Month.Year == SelectedMonth.Year && p.AccountId == accountId);
            result = accountInformation != null ? accountInformation.CheckAsReconcilied : false;
            return result;
        }


        protected virtual double GetTheAccountBalanceofSelectedMonth (long accountId,DateTime SelectedMonth)
        {
            double result = 0;
            var accountInformation =   _accountBalanceRepository.FirstOrDefault(p =>p.Month.Month == SelectedMonth.Month && p.Month.Year == SelectedMonth.Year && p.AccountId == accountId);
            result = accountInformation != null ? accountInformation.Balance :  0;
            return result;
        }
        protected virtual double GetTheTrialBalanceofSelectedMonth(long accountId, DateTime SelectedMonth)
        {
            double result = 0;
            var accountInformation = _accountBalanceRepository.FirstOrDefault(p => p.Month.Month == SelectedMonth.Month && p.Month.Year == SelectedMonth.Year && p.AccountId == accountId);
            result = accountInformation != null ? accountInformation.TrialBalance : 0;
            return result;
        }
        protected virtual int GetStatusofSelectedMonth(long accountId, DateTime SelectedMonth)
        {
            int result = 0;
            var accountInformation = _accountBalanceRepository.FirstOrDefault(p => p.Month.Month == SelectedMonth.Month && p.Month.Year == SelectedMonth.Year && p.AccountId == accountId);
           
            if (accountInformation != null)
            {
                result = accountInformation.Status != 0 ? (int)accountInformation.Status : (int)Status.Open;
            }
            else
            {
                result = (int)Status.Open;
            }
            return result;
        }


        protected virtual async Task CreateManagment(CreateOrEditTimeManagementDto input)
        {
            var timeManagement = ObjectMapper.Map<TimeManagement>(input);


            if (AbpSession.TenantId != null)
            {
                timeManagement.TenantId = (int)AbpSession.TenantId;
            }


            await _timeManagementRepository.InsertAsync(timeManagement);
        }

        protected virtual async Task<bool> GetMonthStatus(DateTime dateTime)
        {
            var management = await _timeManagementRepository.FirstOrDefaultAsync(p =>
              p.Month.Month.Equals(dateTime.Month) && p.Month.Year.Equals(dateTime.Year));
            if (management == null)
            {
                if (dateTime.Year.Equals(DateTime.Now.Year) && dateTime.Month.Equals(DateTime.Now.Month))
                {
                    var createManagement = new CreateOrEditTimeManagementDto()
                    {
                        Month = dateTime,
                        Status = false,
                    };
                    await CreateManagment(createManagement);
                }

                return false;
            }
            return management.Status;

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
                account.IsChange = true;
                account.ChangeTime = DateTime.Today.AddMonths(-1);
                await _chartsofAccountRepository.InsertOrUpdateAndGetIdAsync(account);

                input.IsChange = true;
                input.Id = 0;
                return await Create(input);          
            }
            else
            {
                var updatedAccount = ObjectMapper.Map(input, account);
                updatedAccount.ReconciliationType = (ReconciliationType)input.ReconciliationType;
                updatedAccount.AccountType = (AccountType)input.AccountType;
                var accountId = await _chartsofAccountRepository.InsertOrUpdateAndGetIdAsync(updatedAccount);
                return accountId;
            }
           


        }

        protected virtual async Task<long> Create(CreateOrEditChartsofAccountDto input)
        {
            var account = ObjectMapper.Map<ChartofAccounts.ChartofAccounts>(input);
            account.ChangeTime = null;
            if (AbpSession.TenantId != null)
            {
                account.TenantId = (int)AbpSession.TenantId;
            }
            var accountId = _chartsofAccountRepository.InsertAndGetId(account);

            if (input.Reconciled == Dtos.Reconciled.AccruedAmount)
            {
                var Beginningaccount = _chartsofAccountRepository.FirstOrDefault(p => p.AccountNumber == input.LinkedAccountNumber);
                Beginningaccount.LinkedAccountNumber = input.AccountNumber;
                _chartsofAccountRepository.Update(Beginningaccount);
            }


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
            mappedAccount.CreatorUserId = account.CreatorUserId;
            mappedAccount.CreationTime = account.CreationTime;
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

        public async Task ChangeStatus(long accountId, int selectedStatusId,DateTime SelectedMonth)
        {
            var account = await _accountBalanceRepository.FirstOrDefaultAsync(p => p.AccountId ==accountId && SelectedMonth.Month == p.Month.Month && p.Month.Year == SelectedMonth.Year );
            if (account != null)
            {
                account.Status = (Status)selectedStatusId;
                _accountBalanceRepository.Update(account);
            }
            else
            {
                AccountBalance accountBalance = new AccountBalance();
                accountBalance.Month = SelectedMonth;
                accountBalance.AccountId = accountId;
                accountBalance.Status = (Status)selectedStatusId;
                await _accountBalanceRepository.InsertAsync(accountBalance);
            }
        }

        public async Task AddandUpdateBalance(double balance, long id,DateTime month)
        {
            var account = await _accountBalanceRepository.FirstOrDefaultAsync(p=> p.AccountId == id && month.Month == p.Month.Month && month.Year == p.Month.Year );
            if (account != null)
            {
                account.Month = month;
                account.Balance = balance;
                await _accountBalanceRepository.UpdateAsync(account);
            }
            else
            {
                AccountBalance accountBalance = new AccountBalance();
                accountBalance.Month = month;
                accountBalance.AccountId = id;
                accountBalance.Balance = balance;
              await _accountBalanceRepository.InsertAsync(accountBalance);
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
            var accounts = _chartsofAccountRepository.GetAll().Where(x=>x.IsDeleted == false).Include(x => x.Assignee).Include(p => p.AccountSubType);
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
            var accounts = _chartsofAccountRepository.GetAll().Where(x=>x.IsDeleted == false).Include(x => x.Assignee);
            var listToExport = accounts.Select(item => new ChartsOfAccountsTrialBalanceExcellImportDto
            {
                AccountName = item.AccountName,
                AccountNumber = item.AccountNumber,
                //Balance = item.TrialBalance.ToString()
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
                    type = "Asset";
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

        public async Task<bool> AddTrialBalanceInAccount(ChartsOfAccountsTrialBalanceExcellImportDto input)
        {

            var accountInformation = await _chartsofAccountRepository.FirstOrDefaultAsync(x => x.AccountNumber.ToLower() == input.AccountNumber.Trim().ToLower()&& x.IsDeleted == false);
            if (accountInformation != null)
            {
               var TrialBalanceIsExistOrNot = await _accountBalanceRepository.FirstOrDefaultAsync(x => x.AccountId == accountInformation.Id && x.Month.Month == input.selectedMonth.Month && x.Month.Year == input.selectedMonth.Year);
                if (TrialBalanceIsExistOrNot == null)
                {
                    AccountBalance accountBalance = new AccountBalance();
                    accountBalance.AccountId = accountInformation.Id;
                    accountBalance.TrialBalance = Convert.ToDouble(input.Balance);
                    accountBalance.Month = input.selectedMonth;
                    await _accountBalanceRepository.InsertAsync(accountBalance);
                }
                else
                {
                    TrialBalanceIsExistOrNot.TrialBalance = Convert.ToDouble(input.Balance);
                    await _accountBalanceRepository.UpdateAsync(TrialBalanceIsExistOrNot);
                }

              

                return true;
            }

            return false;
        }
        public async Task<double> GetTrialBalanceofAccount(long id,DateTime month)
        {
            double result = 0;
            var accountBalances = await _accountBalanceRepository.FirstOrDefaultAsync(p => p.AccountId == id && month.Month == p.Month.Month && month.Year == p.Month.Year);
            if (accountBalances != null)
            {
                result = accountBalances.TrialBalance;
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
            if (value == 1) return "Itemized";
            else if (value == 2) return "Amortized";
            else if (value == 3) return "NotReconcilied";
            return "";
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

        public async Task RestoreAccount(long id)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.SoftDelete))
            {
                var account = await _chartsofAccountRepository.FirstOrDefaultAsync(id);
                account.IsDeleted = false;
                await _chartsofAccountRepository.UpdateAsync(account);
            }
               
        }

        public async Task<LinkedAccountInfo> GetLinkAccountDetails(long accountId, DateTime month)
        {
            var currentAccount = _chartsofAccountRepository.FirstOrDefault(p => p.Id == accountId);
            var linkedAccount = _chartsofAccountRepository.FirstOrDefault(p => p.AccountNumber == currentAccount.LinkedAccountNumber);
            var linkedAccountBalance = await _accountBalanceRepository.FirstOrDefaultAsync(p => p.AccountId == linkedAccount.Id && month.Month == p.Month.Month && month.Year == p.Month.Year);

            LinkedAccountInfo linkedAccountInfo = new LinkedAccountInfo();
            linkedAccountInfo.Balance = linkedAccountBalance == null ? 0 : linkedAccountBalance.Balance;
            linkedAccountInfo.TrialBalance = linkedAccountBalance == null ? 0 :  linkedAccountBalance.TrialBalance;

            return linkedAccountInfo;

        }

        public async Task CheckAsReconciliedMonthly(long id, DateTime month)
        {
            var account = await _accountBalanceRepository.FirstOrDefaultAsync(p => p.AccountId == id && month.Month == p.Month.Month && month.Year == p.Month.Year);
            if (account != null)
            {
                account.Month = month;
                account.CheckAsReconcilied = true;
                account.Status = Status.Complete;
                await _accountBalanceRepository.UpdateAsync(account);
            }
            else
            {
                AccountBalance accountBalance = new AccountBalance();
                accountBalance.Month = month;
                accountBalance.AccountId = id;
                accountBalance.CheckAsReconcilied = true;
                account.Status = Status.Complete;
                await _accountBalanceRepository.InsertAsync(accountBalance);
            }



        }
    }
}



