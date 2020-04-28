using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Reporting.Dtos;

namespace Zinlo.Reporting
{
    public class TrialBalanceReportingAppService : ZinloAppServiceBase, ITrialBalanceReportingAppService
    {
        private readonly IRepository<ImportsPaths.ImportsPath, long> _importPathsRepository;
        public TrialBalanceReportingAppService(IRepository<ImportsPaths.ImportsPath, long> importPathsRepository)
        {
            _importPathsRepository = importPathsRepository;
        }

        public Task<List<TrialBalanceReportingViewDto>> GetAllTrialBalanceUploadOfMonth(DateTime SelectedMonth)
        {
            throw new NotImplementedException();
        }

        //public async Task<List<TrialBalanceReportingViewDto>> GetAllTrialBalanceUploadOfMonth(DateTime SelectedMonth)
        //{
        //    var query = _importPathsRepository.GetAll().Where(p => p.Type == "TrialBalance" && SelectedMonth.Month == p.CreationTime.Month && SelectedMonth.Year == p.CreationTime.Year).ToList();

        //    var trialBalanceOfMonth = from o in query.ToList()

        //                       select new TrialBalanceReportingViewDto()
        //                       {
        //                           DateTimeOfUpload = o.CreationTime,
        //                           FilePath = o.FilePath,
        //                       };

        //    var result = await trialBalanceOfMonth.ToList();
        //    return result;

        //}

    }
}
 