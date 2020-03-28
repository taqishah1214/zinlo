using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using System.Text;
using System.Threading.Tasks;
using Zinlo.ChartsofAccount.Dtos;
using Abp.Application.Services.Dto;
using Zinlo.Authorization.Users.Profile;
using Zinlo.ClosingChecklist.Dtos;
using NUglify.Helpers;
using Zinlo.Dto;
using Zinlo.ChartsofAccount.Importing;
using Zinlo.Reconciliation;
using Zinlo.Reconciliation.Dtos;

namespace Zinlo.ChartsofAccount
{
    public class ChartsofAccountAppService : ZinloAppServiceBase, IChartsofAccountAppService
    {
        private readonly IRepository<ChartsofAccount, long> _chartsofAccountRepository;
        private readonly IProfileAppService _profileAppService;
        private readonly IChartsOfAccountsListExcelExporter _chartsOfAccountsListExcelExporter;
        private readonly IRepository<Amortization, long> _amortizationRepository;
        private readonly IRepository<Itemization, long> _itemizationRepository;
        private readonly IChartsOfAccountsTrialBalanceExcelExporter _chartsOfAccountsTrialBalanceExcelExporter;





        public ChartsofAccountAppService(IRepository<Itemization, long> itemizationRepository, IRepository<Amortization, long> amortizationRepository, IRepository<ChartsofAccount, long> chartsofAccountRepository, IProfileAppService profileAppService, IChartsOfAccountsListExcelExporter chartsOfAccountsListExcelExporter, IChartsOfAccountsTrialBalanceExcelExporter chartsOfAccountsTrialBalanceExcelExporter)
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
            DateTime now = DateTime.Now;
            var currentDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);

            var query = _chartsofAccountRepository.GetAll().Where(e => e.CreationTime.Month == currentDate.Month && e.CreationTime.Year == currentDate.Year).Include(p => p.AccountSubType).Include(p => p.Assignee)
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.AccountName.Contains(input.Filter))
                 .WhereIf(input.AccountType != 0, e => false || (e.AccountType == (AccountType)input.AccountType))
                 .WhereIf(input.AssigneeId != 0, e => false || (e.AssigneeId == input.AssigneeId));

            List<GetUserWithPicture> getUserWithPictures = new List<GetUserWithPicture>();
            getUserWithPictures = (from o in query.ToList()
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
        public async Task<double> CreateOrEdit(CreateOrEditChartsofAccountDto input)
        {
            if (input.Id == null || input.Id == 0)
            {
                return await Create(input);

            }
            else
            {
                return await Update(input);
            }

        }
        protected virtual async Task<double> Update(CreateOrEditChartsofAccountDto input)
        {
            var account = await _chartsofAccountRepository.FirstOrDefaultAsync((int)input.Id);

            if ((int)account.ReconciliationType != (int)input.ReconciliationType)
            {
                await _chartsofAccountRepository.DeleteAsync(account);
                int previousAccountId = (int)input.Id;
                if ((int)account.ReconciliationType == 1)
                {
                    _itemizationRepository.Delete(await _itemizationRepository.FirstOrDefaultAsync(x => x.ChartsofAccountId == previousAccountId));
                }
                else
                {
                    _amortizationRepository.Delete(await _amortizationRepository.FirstOrDefaultAsync(x => x.ChartsofAccountId == previousAccountId));
                }

                input.Id = 0;
                return await Create(input);
            }
            else
            {
                var updatedAccount = ObjectMapper.Map(input, account);
                updatedAccount.ReconciliationType = (ReconciliationType)input.ReconciliationType;
                updatedAccount.AccountType = (AccountType)input.AccountType;
                double accountId = await _chartsofAccountRepository.InsertOrUpdateAndGetIdAsync(updatedAccount);
                return accountId;
            }

        }

        protected virtual async Task<double> Create(CreateOrEditChartsofAccountDto input)
        {
            var account = ObjectMapper.Map<ChartsofAccount>(input);
            account.Status = (Status)2;
            if (AbpSession.TenantId != null)
            {
                account.TenantId = (int)AbpSession.TenantId;
            }
            double accountId = _chartsofAccountRepository.InsertAndGetId(account);
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
            List<ChartsOfAccountsExcellExporterDto> listToExport = new List<ChartsOfAccountsExcellExporterDto>();
            foreach (var item in accounts)
            {
                ChartsOfAccountsExcellExporterDto chartsOfAccountsExcellExporterDto = new ChartsOfAccountsExcellExporterDto();
                chartsOfAccountsExcellExporterDto.AccountName = item.AccountName;
                chartsOfAccountsExcellExporterDto.AccountNumber = item.AccountNumber;
                chartsOfAccountsExcellExporterDto.AccountType = GetAccounttypeById((int)item.AccountType);
                chartsOfAccountsExcellExporterDto.AssignedUser = item.Assignee.EmailAddress;
                chartsOfAccountsExcellExporterDto.AccountSubType = item.AccountSubType.Title;
                chartsOfAccountsExcellExporterDto.ReconciliationType = GetReconcilationType((int)item.ReconciliationType);
                chartsOfAccountsExcellExporterDto.ReconciliationAs = GetReconcilationAsValue((int)item.Reconciled);


                listToExport.Add(chartsOfAccountsExcellExporterDto);
            }
            return _chartsOfAccountsListExcelExporter.ExportToFile(listToExport);
        }
        public async Task<FileDto> LoadChartsofAccountTrialBalanceToExcel()
        {
            var accounts = _chartsofAccountRepository.GetAll().Include(x => x.Assignee);
            List<ChartsOfAccountsTrialBalanceExcellImportDto> listToExport = new List<ChartsOfAccountsTrialBalanceExcellImportDto>();
            foreach (var item in accounts)
            {
                ChartsOfAccountsTrialBalanceExcellImportDto model = new ChartsOfAccountsTrialBalanceExcellImportDto();
                model.AccountName = item.AccountName;
                model.AccountNumber = item.AccountNumber;
                model.Balance = item.TrialBalance.ToString();
                listToExport.Add(model);
            }
            return _chartsOfAccountsTrialBalanceExcelExporter.ExportToExcell(listToExport);
        }
        public string GetAccounttypeById(int id)
        {
            string type;
            switch (id)
            {
                case 1:
                    type = "Fixed";
                    break;
                case 2:
                    type = "Assets";
                    break;
                case 3:
                    type = "Liability";
                    break;

                default:
                    type = "Fixed";
                    break;
            }
            return type;
        }

        public async Task<bool> CheckAccountForTrialBalance(ChartsOfAccountsTrialBalanceExcellImportDto input)
        {
            var result = _chartsofAccountRepository.GetAll()
                        .Where(x => x.AccountName.ToLower() == input.AccountName.Trim().ToLower()
                        && x.AccountNumber.ToLower() == input.AccountNumber.Trim().ToLower())
                        // && CompareDates(x.CreationTime) == 0)                       
                        .FirstOrDefault();
            if (result != null)
            {
                result.TrialBalance = Convert.ToDecimal(input.Balance);
                result.VersionId = input.VersionId;
                await _chartsofAccountRepository.UpdateAsync(result);

                return true;
            }
            else
            {
                return false;
            }
        }
        public int CompareDates(DateTime creationDate)
        {
            DateTime dateTime = DateTime.Now;
            DateTime date1 = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
            DateTime date2 = new DateTime(creationDate.Year, creationDate.Month, 1, 0, 0, 0);
            int result = DateTime.Compare(date2, date1);
            return result;
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

        public bool CheckAccountNoExist(string accountNumber)
        {

            if (accountNumber != null)
            {
                bool isExist = _chartsofAccountRepository.GetAll().Any(x => x.AccountNumber.Trim().ToLower() == accountNumber.Trim().ToLower());
                return isExist;
            }
            else
            {
                return false;
            }



        }

        public bool CheckAccounts()
        {
            var result = _chartsofAccountRepository.GetAll().Count();
            if (result > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string GetReconcilationType(int value)
        {
            if (value == 1)
            {
                return "Itemized";
            }
            else
            {
                return "Amortization";
            }

        }
        public async Task ShiftAmortizedItems(double previousAccountId, double newAccountId, DateTime closingMonth)
        {
            var amortizedItemList = _amortizationRepository.GetAll().Where(e => e.ChartsofAccount.Id == previousAccountId).Include(e => e.ChartsofAccount);
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
                                CreationTime = closingMonth,
                                Criteria = (Reconciliation.Dtos.Criteria)o.Criteria,
                                CreatorUserId = o.CreatorUserId,
                            }).ToList();

            foreach (var item in itemList)
            {
                var newItem = ObjectMapper.Map<Amortization>(item);
                await _amortizationRepository.InsertAsync(newItem);
            }

        }



        public async Task ShiftItemizedItem(double previousAccountId, double newAccountId, DateTime closingMonth)
        {
            var itemizedItemList = _itemizationRepository.GetAll().Where(e => e.ChartsofAccount.Id == previousAccountId).Include(e => e.ChartsofAccount);
            var itemList = (from o in itemizedItemList.ToList()

                            select new CreateOrEditItemizationDto()
                            {
                                Id = 0,
                                InoviceNo = o.InoviceNo,
                                JournalEntryNo = o.JournalEntryNo,
                                Date = o.Date,
                                Amount = o.Amount,
                                Description = o.Description,
                                ChartsofAccountId = (long)newAccountId,
                                CreationTime = closingMonth,
                                CreatorUserId = o.CreatorUserId,
                            }).ToList();

            foreach (var item in itemList)
            {
                var newitem = ObjectMapper.Map<Itemization>(item);
                await _itemizationRepository.InsertAsync(newitem);
            }

        }

        public async Task ShiftChartsOfAccountToSpecficMonth(DateTime ClosingMonth)
        {
            var accountsExistCheck = await _chartsofAccountRepository.FirstOrDefaultAsync(e => e.CreationTime.Month == ClosingMonth.Month);
            if (accountsExistCheck == null)
            {
                var currentMonthAccounts = _chartsofAccountRepository.GetAll().Where(e => e.CreationTime.Month == DateTime.Now.Month).Include(a => a.Assignee).Include(a => a.AccountSubType);
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
                                    CreationTime = ClosingMonth,
                                    AssigneeId = o.Assignee.Id
                                }).ToList();

                foreach (var item in itemList)
                {
                    double PreviousAccountId = (double)item.Id;
                    item.Id = 0;
                    double newAccountId = await CreateOrEdit(item);
                    if ((int)item.ReconciliationType == 1)
                    {
                        await ShiftItemizedItem(PreviousAccountId, newAccountId, ClosingMonth);
                    }
                    else
                    {
                        await ShiftAmortizedItems(PreviousAccountId, newAccountId, ClosingMonth);
                    }
                }
            }
        }
        public string GetReconcilationAsValue(int value)
        {
            if (value == 1)
            {
                return "NetAmount";
            }
            else if (value == 2)
            {
                return "BeginningAmount";
            }
            else if (value == 3)
            {

                return "AccruedAmount";
            }
            else
            {
                return string.Empty;
            }

        }
    }
}



