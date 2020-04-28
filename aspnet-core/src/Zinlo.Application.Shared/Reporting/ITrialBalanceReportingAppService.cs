using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Reporting.Dtos;

namespace Zinlo.Reporting
{
   public interface ITrialBalanceReportingAppService
    {
        Task<List<TrialBalanceReportingViewDto>> GetAllTrialBalanceUploadOfMonth(DateTime SelectedMonth);
    }
}
