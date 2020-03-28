using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.ExceptionLogger.Dto;

namespace Zinlo.ExceptionLogger
{
   public interface IExceptionLoggerAppService
    {
        Task<PagedResultDto<ExceptionLoggerForViewDto>> GetAll(GetAllExceptionsInput input);
        Task RollBackTrialBalance(long Id);
    }
}
