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

namespace Zinlo.Contactus
{
    public class ContactusService : ZinloAppServiceBase,IContactusService
    {

        private readonly IRepository<ContactUs, long> _contactusRepository;

        public ContactusService(
            IRepository<ContactUs, long> contactusRepository
            
            )
        {
            _contactusRepository = contactusRepository;

        }
        public async Task Create(CreateOrUpdateContactusInput create,int tenantId)
        {
          var response= await _contactusRepository.InsertAsync(new ContactUs {
           TenantId = tenantId,
            Commitment = 0,
             CompanyName = create.CompanyName,
               Description= create.Description,
                Email= create.Email,
                 FullName= create.FullName,
                  NumberOfUsers = create.NumberOfUsers,
                   Pricing=0
                    
          }).ConfigureAwait(false);
            
        }

        public async Task<ContactusDto> GetContactusByTenantId(GetContactusInput contactusInput)
        {
           var response=  _contactusRepository.GetAll().FirstOrDefault(x => x.TenantId == contactusInput.TenantId);
            if (response == null) return null;

           return new ContactusDto
            {
                Commitment = response.Commitment,
                 CompanyName= response.CompanyName,
                  Description = response.Description ,
                   Email = response.Email,
                    FullName = response.FullName,
                     NumberOfUsers = response.NumberOfUsers,
                      Id = response.Id,
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
                contectusList.Select(response => new ContactusDto {
                    Commitment = response.Commitment,
                    CompanyName = response.CompanyName,
                    Description = response.Description,
                    Email = response.Email,
                    FullName = response.FullName,
                    NumberOfUsers = response.NumberOfUsers,
                    Id = response.Id,
                    TenantId =response.TenantId,
                     IsAccepted = response.IsAccepted,
                     Pricing = response.Pricing,
                     CreationTime =response.CreationTime
                }).ToList()
                );
        }
    }
}
