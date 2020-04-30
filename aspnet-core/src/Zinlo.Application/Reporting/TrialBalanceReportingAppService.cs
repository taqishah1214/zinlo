using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zinlo.ErrorLog.Dto;
using Zinlo.ImportLog.Dto;
using Zinlo.Reporting.Dtos;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;


namespace Zinlo.Reporting
{
    public class TrialBalanceReportingAppService : ZinloAppServiceBase, ITrialBalanceReportingAppService
    {
        private readonly IRepository<ImportsPaths.ImportsPath, long> _importPathsRepository;
        public TrialBalanceReportingAppService(IRepository<ImportsPaths.ImportsPath, long> importPathsRepository)
        {
            _importPathsRepository = importPathsRepository;
        }

        public async Task<PagedResultDto<ImportLogForViewDto>> GetAll(GetAllImportLogInput input)
        {
            var query = _importPathsRepository.GetAll().Where(p => p.Type == "TrialBalance").Include(p => p.User)
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.FilePath.Contains(input.Filter));

            var pagedAndFilteredAccounts = query.OrderBy(input.Sorting ?? "CreationTime desc").PageBy(input);
            var totalCount = query.Count();
            var accountsList = from o in pagedAndFilteredAccounts.ToList()

                               select new ImportLogForViewDto()
                               {
                                   Id = o.Id,
                                   Type = o.Type,
                                   FilePath = o.UploadedFilePath,
                                   CreationTime = o.CreationTime,
                                   Records = o.SuccessRecordsCount + "/" + (o.FailedRecordsCount + o.SuccessRecordsCount),
                                   CreatedById = o.User.Id,
                                   IsRollBacked = o.IsRollBacked,
                                   SuccessFilePath = o.SuccessFilePath != "" ? o.SuccessFilePath : ""
                               };

            return new PagedResultDto<ImportLogForViewDto>(
               totalCount,
               accountsList.ToList()
           );

        }


        public async Task<List<GetTrialBalanceofSpecficMonth>> GetTrialBalancesofSpecficMonth(DateTime SelectedMonth)
        {
            var query = _importPathsRepository.GetAll().Where(p => p.Type == "TrialBalance" && SelectedMonth.Month == p.CreationTime.Month && SelectedMonth.Year == p.CreationTime.Year).ToList();

            var trialBalanceOfMonth = from o in query.ToList()
                                      select new GetTrialBalanceofSpecficMonth()
                                      {
                                          id = o.Id,
                                          CreationTime = o.CreationTime,
                                          Name = o.UploadedFilePath,
                                      };

            return trialBalanceOfMonth.ToList();

        }

       


    }
}
 