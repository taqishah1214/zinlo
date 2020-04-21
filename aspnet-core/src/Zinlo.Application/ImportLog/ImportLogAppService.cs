using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Zinlo.ErrorLog.Dto;
using Zinlo.ImportLog.Dto;
using Zinlo.ImportsPaths;

namespace Zinlo.ImportLog
{
    public class ImportLogAppService : ZinloAppServiceBase, IImportLogAppService
    {
        private readonly IRepository<ImportsPath, long> _importsPathRepository;
        private readonly IRepository<ChartofAccounts.ChartofAccounts, long> _chartOfAccountRepository;
        public ImportLogAppService(IRepository<ImportsPath, long> importsPathRepository,
            IRepository<ChartofAccounts.ChartofAccounts, long> chartOfAccountRepository)
        {
            _importsPathRepository = importsPathRepository;
            _chartOfAccountRepository = chartOfAccountRepository;
        }

        public async Task<PagedResultDto<ImportLogForViewDto>> GetAll(GetAllImportLogInput input)
        {
            var query = _importsPathRepository.GetAll().Include(p => p.User)
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.FilePath.Contains(input.Filter));

            var pagedAndFilteredAccounts = query.OrderBy(input.Sorting ?? "CreationTime desc").PageBy(input);
            var totalCount = query.Count();
            var accountsList = from o in pagedAndFilteredAccounts.ToList()

                               select new ImportLogForViewDto()
                               {
                                   Id = o.Id,
                                   Type = o.Type,
                                   FilePath = o.FilePath != "" ? o.FilePath : "",
                                   CreationTime = o.CreationTime,
                                   Records = o.SuccessRecordsCount + "/" + (o.FailedRecordsCount + o.SuccessRecordsCount),
                                   CreatedBy = o.User.EmailAddress,
                                   IsRollBacked = o.IsRollBacked,
                                   SuccessFilePath = o.SuccessFilePath != ""? o.SuccessFilePath: ""
                               };

            return new PagedResultDto<ImportLogForViewDto>(
               totalCount,
               accountsList.ToList()
           );

        }

        public async Task RollBackTrialBalance(long id)
        {
            //var result = _chartOfAccountRepository.GetAll().Where(x => x.VersionId == id).ToList();
            //foreach (var item in result)
            //{
            //    //item.TrialBalance = 0;
            //    //item.VersionId = 0;
            //    await _chartOfAccountRepository.UpdateAsync(item);
            //}
            //var versionFile = _importsPathRepository.FirstOrDefault(id);
            //versionFile.IsRollBacked = true;
            //_importsPathRepository.Update(versionFile);
        }
    }
}
