using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Linq.Extensions;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Zinlo.AccountSubType.Dtos;
using Zinlo.Authorization.Users;
using Zinlo.Authorization.Users.Profile;
using Microsoft.EntityFrameworkCore;

namespace Zinlo.AccountSubType
{
    public class AccountSubTypeAppService : ZinloAppServiceBase, IAccountSubTypeAppService
    {
        private readonly IRepository<AccountSubType, long> _accountSubTypeRepository;
        private readonly UserManager _userManager;
        private readonly IProfileAppService _profileAppService;

        public AccountSubTypeAppService (IRepository<AccountSubType, long> accountSubTypeRepository, UserManager userManager, IRepository<User, long> userRepository, IProfileAppService profileAppService)
        {
            _accountSubTypeRepository = accountSubTypeRepository;
            _userManager = userManager;
            _profileAppService = profileAppService;
        }

        public async Task CreateOrEdit(CreateOrEditAccountSubTypeDto input)
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

        public async Task<CreateOrEditAccountSubTypeDto> GetAccountSubTypeForEdit(long id)
        {
            var accountSubType = await _accountSubTypeRepository.FirstOrDefaultAsync(id);

            var output = ObjectMapper.Map<CreateOrEditAccountSubTypeDto>(accountSubType);

            return output;
        }

        private async Task Update(CreateOrEditAccountSubTypeDto input)
        {
            var category = await _accountSubTypeRepository.FirstOrDefaultAsync((int)input.Id);
            ObjectMapper.Map(input, category);
        }
        protected virtual async Task Create(CreateOrEditAccountSubTypeDto input)
        {
            var accountSubType = ObjectMapper.Map<AccountSubType>(input);
            if (AbpSession.TenantId != null)
            {
                accountSubType.TenantId = (int)AbpSession.TenantId;
            }

            await _accountSubTypeRepository.InsertAsync(accountSubType);
        }

        public async Task Delete(long id)
        {
            await _accountSubTypeRepository.DeleteAsync(id);
        }

        public async Task<PagedResultDto<GetAccountSubTypeForViewDto>> GetAll(GetAllAccountSubTypeInput input)
        {
            var filteredAccountSubTypes = _accountSubTypeRepository.GetAll()
                        .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Title.Contains(input.Filter) || e.Description.Contains(input.Filter))
                        .WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => e.Title == input.TitleFilter)
                        .WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Description == input.DescriptionFilter);

            var pagedAndFilteredAccountSubTypes = filteredAccountSubTypes.OrderBy(input.Sorting ?? "id asc").PageBy(input);

            var totalCount = await filteredAccountSubTypes.CountAsync();
            var mappedData = ObjectMapper.Map<List<GetAccountSubTypeForViewDto>>(pagedAndFilteredAccountSubTypes);
            foreach (var data in mappedData)
            {
                var userDetail = UserManager.GetUserById((long)data.UserId);
                data.CreatedBy = userDetail.FullName;
                data.ProfilePicture = userDetail.ProfilePictureId.HasValue ? "data:image/jpeg;base64," + _profileAppService.GetProfilePictureById((Guid)userDetail.ProfilePictureId).Result.ProfilePicture : "";
            }
            return new PagedResultDto<GetAccountSubTypeForViewDto>(
                totalCount,
                 mappedData
            );
        }


    }
}
