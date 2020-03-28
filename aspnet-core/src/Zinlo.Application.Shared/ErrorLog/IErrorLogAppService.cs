using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Zinlo.ErrorLog.Dto;

namespace Zinlo.ErrorLog
{
   public interface IErrorLogAppService : IApplicationService
    {
        Task<PagedResultDto<ErrorLogForViewDto>> GetAll(GetAllErroLogInput input);
    }
}
