using System.Threading.Tasks;
using Abp.Application.Services;
using Zinlo.Configuration.Host.Dto;

namespace Zinlo.Configuration.Host
{
    public interface IHostSettingsAppService : IApplicationService
    {
        Task<HostSettingsEditDto> GetAllSettings();

        Task UpdateAllSettings(HostSettingsEditDto input);

        Task SendTestEmail(SendTestEmailInput input);
    }
}
