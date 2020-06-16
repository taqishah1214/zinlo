using Abp.Application.Services;
using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Contactus.Dto;

namespace Zinlo.Contactus
{
    public interface IContactusService:IApplicationService
    {

        
         Task<PagedResultDto<ContactusDto>> GetContectus(GetContactusListInput input);
        Task<ContactusDto> GetContactusByTenantId(GetContactusInput nput);
        Task Create(CreateOrUpdateContactusInput create,int tenantId);
        Task<bool> ApproveRequest(ContactusDto contactus);
        Task DeteleByTenantId(int tenantId);
    }
}
