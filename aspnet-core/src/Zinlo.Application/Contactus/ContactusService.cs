using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Authorization.Users;
using Zinlo.Contactus.Dto;

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
            return ObjectMapper.Map<ContactusDto>(_contactusRepository.GetAll().FirstOrDefault(x => x.TenantId == contactusInput.TenantId));
        }
    }
}
