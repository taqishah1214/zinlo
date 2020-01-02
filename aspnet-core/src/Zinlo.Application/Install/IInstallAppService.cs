using System.Threading.Tasks;
using Abp.Application.Services;
using Zinlo.Install.Dto;

namespace Zinlo.Install
{
    public interface IInstallAppService : IApplicationService
    {
        Task Setup(InstallDto input);

        AppSettingsJsonDto GetAppSettingsJson();

        CheckDatabaseOutput CheckDatabase();
    }
}