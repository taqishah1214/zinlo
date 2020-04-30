using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Zinlo.Authorization.Users;
using Zinlo.Contactus.Dto;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Zinlo.MultiTenancy;
using Abp.Domain.Uow;
using Zinlo.Url;
using Zinlo.Authorization.Accounts.Dto;
using Abp.Runtime.Security;
using System.Web;
using System;

namespace Zinlo.Contactus
{
    public class ContactusService : ZinloAppServiceBase, IContactusService
    {
        private readonly ICurrentUnitOfWorkProvider _unitOfWorkProvider;
        private readonly IRepository<ContactUs, long> _contactusRepository;
        private readonly IUserEmailer _userEmailer;
        private readonly IRepository<Tenant> _tenantRepository;
        public IAppUrlService AppUrlService { get; set; }
        public ContactusService(
            IRepository<ContactUs, long> contactusRepository,
              IUserEmailer userEmailer,
               IRepository<Tenant> tenantRepository,
               ICurrentUnitOfWorkProvider unitOfWorkProvider
            )
        {
            _contactusRepository = contactusRepository;
            _userEmailer = userEmailer;
            _tenantRepository = tenantRepository;
            _unitOfWorkProvider = unitOfWorkProvider;
            AppUrlService = NullAppUrlService.Instance;
        }

        public async Task<bool> ApproveRequest(ContactusDto contactus)
        {
            
            
            var response = _contactusRepository.GetAll().FirstOrDefault(x => x.TenantId == contactus.TenantId);
            response.Pricing = contactus.Pricing;
            response.IsAccepted = true;
            response.Commitment = contactus.Commitment;
            await _contactusRepository.UpdateAsync(response);
            var res = _tenantRepository.FirstOrDefault(x => x.Id == response.TenantId);
            await _userEmailer.SendCustomPlaEmail(response.Email, "" + response.Pricing, AppUrlService.CreateCustomPlanUrlFormat(response.TenantId), response.TenantId, res.EditionId.Value, 4, 0,response.Commitment);
            return true;
        }

        private string GetTenancyNameOrNull(int? tenantId)
        {
            if (tenantId == null)
            {
                return null;
            }

            using (_unitOfWorkProvider.Current.SetTenantId(null))
            {

                return _tenantRepository.Get(tenantId.Value).TenancyName;

            }

        }
        public async Task Create(CreateOrUpdateContactusInput create, int tenantId)
        {
            var response = await _contactusRepository.InsertAsync(new ContactUs
            {
                TenantId = tenantId,
                Commitment = 0,
                CompanyName = create.CompanyName,
                Description = create.Description,
                Email = create.Email,
                FullName = create.FullName,
                NumberOfUsers = create.NumberOfUsers,
                Pricing = 0

            }).ConfigureAwait(false);

        }

        public async Task<ContactusDto> GetContactusByTenantId(GetContactusInput contactusInput)
        {
            var response = _contactusRepository.GetAll().FirstOrDefault(x => x.TenantId == contactusInput.TenantId);
            if (response == null) return null;

            return new ContactusDto
            {
                Commitment = response.Commitment,
                CompanyName = response.CompanyName,
                Description = response.Description,
                Email = response.Email,
                FullName = response.FullName,
                NumberOfUsers = response.NumberOfUsers,
                Id = response.Id,
                TenantName = GetTenancyNameOrNull(response.TenantId),
                TenantId = response.TenantId,
                IsAccepted = response.IsAccepted,
                Pricing = response.Pricing,
                CreationTime = response.CreationTime
            };
        }
       

        public async Task<PagedResultDto<ContactusDto>> GetContectus(GetContactusListInput input)
        {
            var query = _contactusRepository.GetAll();
            var contectsCount = await query.CountAsync();
            var contectusList = await query.OrderBy("id").PageBy(input).ToListAsync();
            return new PagedResultDto<ContactusDto>(
                contectsCount,
                contectusList.Select(response => new ContactusDto
                {
                    Commitment = response.Commitment,
                    CompanyName = response.CompanyName,
                    Description = response.Description,
                    Email = response.Email,
                    TenantName = GetTenancyNameOrNull(response.TenantId),
                    FullName = response.FullName,
                    NumberOfUsers = response.NumberOfUsers,
                    Id = response.Id,
                    TenantId = response.TenantId,
                    IsAccepted = response.IsAccepted,
                    Pricing = response.Pricing,
                    CreationTime = response.CreationTime
                }).ToList()
                );
        }
    }
}
