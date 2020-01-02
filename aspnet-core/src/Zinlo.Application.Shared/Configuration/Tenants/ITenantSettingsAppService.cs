using System.Threading.Tasks;
using Abp.Application.Services;
using Zinlo.Configuration.Tenants.Dto;

namespace Zinlo.Configuration.Tenants
{
    public interface ITenantSettingsAppService : IApplicationService
    {
        Task<TenantSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(TenantSettingsEditDto input);

        Task ClearLogo();

        Task ClearCustomCss();
    }
}
