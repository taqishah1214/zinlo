using Abp.Application.Services;
using Zinlo.Dto;
using Zinlo.Logging.Dto;

namespace Zinlo.Logging
{
    public interface IWebLogAppService : IApplicationService
    {
        GetLatestWebLogsOutput GetLatestWebLogs();

        FileDto DownloadWebLogs();
    }
}
