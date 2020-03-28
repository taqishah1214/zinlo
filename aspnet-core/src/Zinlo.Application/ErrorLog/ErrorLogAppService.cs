using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Zinlo.ImportsPaths;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Zinlo.Authorization.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Zinlo.Configuration;
using Zinlo.ErrorLog.Dto;

namespace Zinlo.ErrorLog
{
    public class ErrorLogAppService : ZinloAppServiceBase, IErrorLogAppService
    {
        private readonly IRepository<ImportsPath, long> _importsPathRepository;
        private readonly IRepository<ChartsofAccount.ChartsofAccount, long> _chartOfAccountRepository;
        private readonly IConfigurationRoot _appConfiguration;
        public UserManager userManager { get; set; }
        public ErrorLogAppService(IRepository<ImportsPath, long> importsPathRepository,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            IRepository<ChartsofAccount.ChartsofAccount, long> chartOfAccountRepository,
        IWebHostEnvironment env
            )
        {
            _importsPathRepository = importsPathRepository;
            _appConfiguration = env.GetAppConfiguration();
            _chartOfAccountRepository = chartOfAccountRepository;
        }

        public async Task<PagedResultDto<ErrorLogForViewDto>> GetAll(GetAllErroLogInput input)
        {
            var query = _importsPathRepository.GetAll().Include(p => p.User)
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.FilePath.Contains(input.Filter));

            var pagedAndFilteredAccounts = query.OrderBy(input.Sorting ?? "CreationTime desc").PageBy(input);
            var totalCount = query.Count();
            var accountsList = from o in pagedAndFilteredAccounts.ToList()

                               select new ErrorLogForViewDto()
                               {
                                   Id = o.Id,
                                   Type = o.Type,
                                   FilePath = o.FilePath != "" ? o.FilePath : "",
                                   CreationTime = o.CreationTime,
                                   Records = o.SuccessRecordsCount + "/" + (o.FailedRecordsCount + o.SuccessRecordsCount),
                                   CreatedBy = o.User.EmailAddress,
                                   IsRollBacked = o.IsRollBacked
                               };

            return new PagedResultDto<ErrorLogForViewDto>(
               totalCount,
               accountsList.ToList()
           );

        }

        public async Task RollBackTrialBalance(long id)
        {
            var result = _chartOfAccountRepository.GetAll().Where(x => x.VersionId == id).ToList();
            foreach (var item in result)
            {
                item.TrialBalance = 0;
                item.VersionId = 0;
                await _chartOfAccountRepository.UpdateAsync(item);
            }
            var versionFile = _importsPathRepository.FirstOrDefault(id);
            versionFile.IsRollBacked = true;
            _importsPathRepository.Update(versionFile);
        }
    }
}
