using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Contactus.Dto;

namespace Zinlo.Contactus
{
    public interface IContactusService:IApplicationService
    {
        Task<ContactusDto> GetContactusByTenantId(GetContactusInput contactusInput);
        Task Create(CreateOrUpdateContactusInput create,int tenantId);
    }
}
