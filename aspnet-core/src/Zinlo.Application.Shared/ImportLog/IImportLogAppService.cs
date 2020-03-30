using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Zinlo.ErrorLog.Dto;
using Zinlo.ImportLog.Dto;

namespace Zinlo.ImportLog
{
    public interface IImportLogAppService
    {
        Task<PagedResultDto<ImportLogForViewDto>> GetAll(GetAllImportLogInput input);
        Task RollBackTrialBalance(long id);
    }
}