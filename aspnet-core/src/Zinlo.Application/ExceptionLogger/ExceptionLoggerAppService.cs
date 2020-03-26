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
using Microsoft.AspNetCore.Identity;
using Zinlo.Authorization.Users;
using AutoMapper.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Zinlo.Configuration;

namespace Zinlo.ExceptionLogger
{
    public class ExceptionLoggerAppService : ZinloAppServiceBase, IExceptionLoggerAppService
    {
        private readonly IRepository<ImportsPath, long> _importsPathRepository;
        private readonly IConfigurationRoot _appConfiguration;
        public UserManager userManager { get; set; }
        public ExceptionLoggerAppService(IRepository<ImportsPath, long> importsPathRepository,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            IWebHostEnvironment env
            )
        {
            _importsPathRepository = importsPathRepository;
            _appConfiguration = env.GetAppConfiguration();
        }
      
        public async Task<PagedResultDto<ExceptionLoggerForViewDto>> GetAll(GetAllExceptionsInput input)
        {
            var query = _importsPathRepository.GetAll().Include(p => p.User)
                 .WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.FilePath.Contains(input.Filter));

            var pagedAndFilteredAccounts = query.OrderBy(input.Sorting ?? "CreationTime asc").PageBy(input);
            var totalCount = query.Count();
            var baseUrl = _appConfiguration["App:ServerRootAddress"];
            var accountsList = from o in pagedAndFilteredAccounts.ToList()
                               
                               select new ExceptionLoggerForViewDto()
                               {
                                   Id = o.Id,
                                   Type = o.Type,
                                   FilePath = baseUrl + o.FilePath,
                                   CreationTime = o.CreationTime,
                                   Records = o.SuccessRecordsCount + "/" + (o.FailedRecordsCount + o.SuccessRecordsCount).ToString(),
                                   CreatedBy = o.User.FullName
                               };

            return new PagedResultDto<ExceptionLoggerForViewDto>(
               totalCount,
               accountsList.ToList()
           );

        }
    }
}
