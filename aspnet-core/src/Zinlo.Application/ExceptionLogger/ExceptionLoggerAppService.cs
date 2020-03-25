using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using System.Linq;
using System.Threading.Tasks;
using Zinlo.ExceptionLogger.Dto;
using Zinlo.ImportsPaths;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;


namespace Zinlo.ExceptionLogger
{
    public class ExceptionLoggerAppService : ZinloAppServiceBase, IExceptionLoggerAppService
    {
        private readonly IRepository<ImportsPath, long> _importsPathRepository;
        public ExceptionLoggerAppService(IRepository<ImportsPath, long> importsPathRepository)
        {
            _importsPathRepository = importsPathRepository;
        }
      
        public async Task<PagedResultDto<ExceptionLoggerForViewDto>> GetAll(GetAllExceptionsInput input)
        {
            var query = _importsPathRepository.GetAll().Include(p => p.User)
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.FilePath.Contains(input.Filter));

            var pagedAndFilteredAccounts = query.OrderBy(input.Sorting ?? "id asc").PageBy(input);
            var totalCount = query.Count();

            var accountsList = from o in pagedAndFilteredAccounts

                               select new ExceptionLoggerForViewDto()
                               {
                                   Id = o.Id,
                                   Type = o.Type,
                                   FilePath = o.FilePath,
                                   CreationTime = o.CreationTime,
                                   FailedRecordsCount = o.FailedRecordsCount,
                                   SuccessRecordsCount = o.SuccessRecordsCount,
                                   CreatedBy = o.User.FullName                                 
                               };

            return new PagedResultDto<ExceptionLoggerForViewDto>(
               totalCount,
               accountsList.ToList()
           );

        }
    }
}
