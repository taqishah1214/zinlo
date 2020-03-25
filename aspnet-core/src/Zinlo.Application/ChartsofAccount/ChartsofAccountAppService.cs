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

namespace Zinlo.ChartsofAccount
{
    public class ChartsofAccountAppService : ZinloAppServiceBase, IChartsofAccountAppService
    {
        private readonly IRepository<ChartsofAccount, long> _chartsofAccountRepository;
        private readonly IProfileAppService _profileAppService;
        private readonly IChartsOfAccountsListExcelExporter _chartsOfAccountsListExcelExporter;
        private readonly IChartsOfAccountsTrialBalanceExcelExporter _chartsOfAccountsTrialBalanceExcelExporter;
        public ChartsofAccountAppService(IRepository<ChartsofAccount, long> chartsofAccountRepository, IProfileAppService profileAppService,
            IChartsOfAccountsListExcelExporter chartsOfAccountsListExcelExporter,
            IChartsOfAccountsTrialBalanceExcelExporter chartsOfAccountsTrialBalanceExcelExporter
            )
        {
            _chartsofAccountRepository = chartsofAccountRepository;
            _profileAppService = profileAppService;
            _chartsOfAccountsListExcelExporter = chartsOfAccountsListExcelExporter;
            _chartsOfAccountsTrialBalanceExcelExporter = chartsOfAccountsTrialBalanceExcelExporter;
        }

        
        public async Task<PagedResultDto<ChartsofAccoutsForViewDto>> GetAll(GetAllChartsofAccountInput input)
        {
            DateTime now = DateTime.Now;
            var CurrentDate = new DateTime(now.Year, now.Month, 1);

            var query = _chartsofAccountRepository.GetAll().Where(e => e.CreationTime.Month == CurrentDate.Month && e.CreationTime.Year == CurrentDate.Year).Include(p => p.AccountSubType).Include(p => p.Assignee)
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
                                   ProfilePicture = o.Assignee.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)o.Assignee.ProfilePictureId).Result.ProfilePicture : "",
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
        public async Task CreateOrEdit(CreateOrEditChartsofAccountDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }

        }
        protected virtual async Task Update(CreateOrEditChartsofAccountDto input)
        {
            var account = await _chartsofAccountRepository.FirstOrDefaultAsync((int)input.Id);
            if ((int)account.ReconciliationType != (int)input.ReconciliationType)
            {
                int previousId = (int)input.Id;
                input.Id = 0;

                await Create(input);
                var recociliationTypeOld = (ReconciliationType)account.ReconciliationType;
                var updatedAccount = ObjectMapper.Map(input, account);
                updatedAccount.ReconciliationType = recociliationTypeOld;
                updatedAccount.AccountType = (AccountType)input.AccountType;
                updatedAccount.Lock = true;
                updatedAccount.Id = previousId;
                await _chartsofAccountRepository.UpdateAsync(updatedAccount);
            }
            else
            {
                var updatedAccount = ObjectMapper.Map(input, account);
                updatedAccount.ReconciliationType = (ReconciliationType)input.ReconciliationType;
                updatedAccount.AccountType = (AccountType)input.AccountType;
                await _chartsofAccountRepository.UpdateAsync(updatedAccount);
            }
               
        }





        protected virtual async Task Create(CreateOrEditChartsofAccountDto input)
        {
            var account = ObjectMapper.Map<ChartsofAccount>(input);
            account.Status = (Status)2;
            if (AbpSession.TenantId != null)
            {
                account.TenantId = (int)AbpSession.TenantId;
            }
            await _chartsofAccountRepository.InsertAsync(account);
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
            var accounts =  _chartsofAccountRepository.GetAll().Include(x => x.Assignee);
            List<ChartsOfAccountsExcellExporterDto> listToExport = new List<ChartsOfAccountsExcellExporterDto>();
            foreach (var item in accounts)
            {
                ChartsOfAccountsExcellExporterDto chartsOfAccountsExcellExporterDto = new ChartsOfAccountsExcellExporterDto();
                chartsOfAccountsExcellExporterDto.AccountName = item.AccountName;
                chartsOfAccountsExcellExporterDto.AccountNumber = item.AccountNumber;
                chartsOfAccountsExcellExporterDto.AccountType = GetAccounttypeById((int)item.AccountType);
                chartsOfAccountsExcellExporterDto.AssignedUser = item.Assignee.FullName;
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
            string type = string.Empty;
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

        public async Task<bool> CheckAccountForTrialBalance(string accountName, string accountNumber,string trialBalance)
        {
            var result = _chartsofAccountRepository.GetAll()
                        .Where(x => x.AccountName.ToLower() == accountName.Trim().ToLower()
                        && x.AccountNumber.ToLower() == accountNumber.Trim().ToLower())
                       // && CompareDates(x.CreationTime) == 0)                       
                        .FirstOrDefault();
            if(result != null)
            {
                result.TrialBalance = Convert.ToDecimal(trialBalance);
               await _chartsofAccountRepository.UpdateAsync(result);     
                
                return true;
            }
            else
            {
                return false;
            }         
        }
        public int CompareDates(DateTime CreattionDate)
        {
            DateTime dateTime = DateTime.Now;
            DateTime date1 = new DateTime(dateTime.Year, dateTime.Month, 1, 0, 0, 0);
            DateTime date2 = new DateTime(CreattionDate.Year, CreattionDate.Month, 1, 0, 0, 0);
            int result = DateTime.Compare(date2, date1);
            return result;
        }
    }
}
