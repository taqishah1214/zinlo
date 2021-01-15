using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Zinlo.Editions.Dto;
using Zinlo.MultiTenancy.Dto;

namespace Zinlo.MultiTenancy
{
    public interface ITenantRegistrationAppService: IApplicationService
    {
        Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input);

        Task<string> CheckTheLinkAndReturnEmail(string Link);

        Task<EditionsSelectOutput> GetEditionsForSelect(string Link);

        Task<EditionSelectDto> GetEdition(int editionId);
        Task<bool> UnRegisterTenant(int tenantId);
        Task<bool> SetTenantExpire(int tenantId,DateTime expireDate);
    }
}