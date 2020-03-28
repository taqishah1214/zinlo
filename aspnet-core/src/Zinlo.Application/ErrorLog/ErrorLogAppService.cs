using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;
using Zinlo.ErrorLog.Dto;
using Zinlo.ImportsPaths;

namespace Zinlo.ErrorLog
{
    public class ErrorLogAppService : ZinloAppServiceBase, IErrorLogAppService
    {
        private readonly IRepository<ImportsPath, long> _importsPathRepository;
        public ErrorLogAppService(IRepository<ImportsPath, long> importsPathRepository
            )
        {
            _importsPathRepository = importsPathRepository;
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
                                   FilePath = o.FilePath,
                                   CreationTime = o.CreationTime,
                                   Records = o.SuccessRecordsCount + "/" + (o.FailedRecordsCount + o.SuccessRecordsCount),
                                   CreatedBy = o.User.FullName
                               };

            return new PagedResultDto<ErrorLogForViewDto>(
               totalCount,
               accountsList.ToList()
           );

        }
    }
}
