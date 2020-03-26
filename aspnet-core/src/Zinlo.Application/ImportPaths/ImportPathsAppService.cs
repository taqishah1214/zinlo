using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.ImportPaths.Dto;
using Zinlo.ImportsPaths;

namespace Zinlo.ImportPaths
{
    public class ImportPathsAppService : ZinloAppServiceBase, IImportPathsAppService
    {
        private readonly IRepository<ImportsPath, long> _importPathsRepository;
        public ImportPathsAppService(IRepository<ImportsPath, long> importPathsRepository)
        {
            _importPathsRepository = importPathsRepository;
        }
        public async Task SaveFilePath(ImportPathDto input)
        {
            ImportsPath importsPath = new ImportsPath();
            importsPath.FilePath = input.FilePath;
            importsPath.TenantId = input.TenantId;
            importsPath.Type = input.Type;
            importsPath.FailedRecordsCount = input.FailedRecordsCount;
            importsPath.SuccessRecordsCount = input.SuccessRecordsCount;
            importsPath.CreatorUserId = input.CreatorId;
            importsPath.UserId = input.CreatorId;
            
            importsPath.CreationTime = DateTime.UtcNow;
           await _importPathsRepository.InsertAsync(importsPath);          

        }
    }
}
