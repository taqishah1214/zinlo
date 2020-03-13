﻿using Abp.Collections.Extensions;
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

namespace Zinlo.ChartsofAccount
{
    public class ChartsofAccountAppService : ZinloAppServiceBase, IChartsofAccountAppService
    {
        private readonly IRepository<ChartsofAccount, long> _chartsofAccountRepository;
        private readonly IProfileAppService _profileAppService;

        public ChartsofAccountAppService(IRepository<ChartsofAccount, long> chartsofAccountRepository, IProfileAppService profileAppService)
        {
            _chartsofAccountRepository = chartsofAccountRepository;
            _profileAppService = profileAppService;
        }

        
        public async Task<PagedResultDto<ChartsofAccoutsForViewDto>> GetAll(GetAllChartsofAccountInput input)
        {
            DateTime now = DateTime.Now;
            var CurrentDate = new DateTime(now.Year, now.Month, 1);

            var query = _chartsofAccountRepository.GetAll().Include(p => p.AccountSubType).Include(p => p.Assignee)
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
    }
}
