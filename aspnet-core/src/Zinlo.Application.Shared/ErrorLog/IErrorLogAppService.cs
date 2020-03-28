using Abp.Application.Services.Dto;
using System.Threading.Tasks;
using Zinlo.ErrorLog.Dto;

namespace Zinlo.ErrorLog
{
    public interface IErrorLogAppService
    {
        Task<PagedResultDto<ErrorLogForViewDto>> GetAll(GetAllErroLogInput input);
        Task RollBackTrialBalance(long id);
    }
}