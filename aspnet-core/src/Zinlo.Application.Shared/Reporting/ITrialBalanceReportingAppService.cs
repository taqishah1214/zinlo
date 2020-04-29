using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.ErrorLog.Dto;
using Zinlo.ImportLog.Dto;
using Zinlo.Reporting.Dtos;

namespace Zinlo.Reporting
{
   public interface ITrialBalanceReportingAppService
    {
        Task<PagedResultDto<ImportLogForViewDto>> GetAll(GetAllImportLogInput input);
        Task<List<GetTrialBalanceofSpecficMonth>> GetTrialBalancesofSpecficMonth(DateTime SelectedMonth);

    }
}
