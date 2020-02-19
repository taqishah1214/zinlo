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

namespace Zinlo.ChartsofAccount
{
   public class ChartsofAccountAppService : ZinloAppServiceBase , IChartsofAccountAppService
    {
        private readonly IRepository<ChartsofAccount, long> _chartsofAccountRepository;
        private readonly IProfileAppService _profileAppService;

        public ChartsofAccountAppService(IRepository<ChartsofAccount, long> chartsofAccountRepository, IProfileAppService profileAppService)
        {
            _chartsofAccountRepository = chartsofAccountRepository;
            _profileAppService = profileAppService;
        }

       public async  Task<PagedResultDto<ChartsofAccoutsForViewDto>> GetAll(GetAllChartsofAccountInput input)
        {
            var query =  _chartsofAccountRepository.GetAll()
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.AccountName.Contains(input.Filter))
                 .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => false || e.AccountNumber.Contains(input.TitleFilter));

            var pagedAndFilteredAccounts = query.OrderBy(input.Sorting ?? "id asc").PageBy(input);
            var totalCount = query.Count();

            var accountsList = from o in pagedAndFilteredAccounts.ToList()

                                   select new ChartsofAccoutsForViewDto()
                                   {
                                       Id = o.Id,
                                       AccountName = o.AccountName,
                                       AccountNumber = o.AccountNumber,
                                       AccountTypeId = (int)o.AccountType,
                                       AccountSubTypeId = o.AccountSubType != null? o.AccountSubType.Id:0,

                                       AccountSubType = o.AccountSubType != null ? o.AccountSubType.Title : "",
                                       ReconciliationTypeId = o.ReconciliationType != 0 ? (int)o.ReconciliationType : 0,
                                       AssigneeName = o.Assignee != null ? o.Assignee.FullName : "",
                                       //ProfilePicture = o.Assignee.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)o.Assignee.ProfilePictureId).Result.ProfilePicture : ""

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
            account.ReconciliationType = (ReconciliationType)input.ReconciliationType;
            account.AccountType = (AccountType)input.AccountType;
            _chartsofAccountRepository.Update(account);

        }

        protected virtual async Task Create(CreateOrEditChartsofAccountDto input)
        {
            var account = ObjectMapper.Map<ChartsofAccount>(input);


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

            return mappedAccount;

        }

      
      
    }
}
