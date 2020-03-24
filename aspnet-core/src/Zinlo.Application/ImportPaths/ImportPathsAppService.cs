using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        public async Task SaveFilePath(string url)
        {
            ImportsPath importsPath = new ImportsPath();
            importsPath.FilePath = url;
            importsPath.TenantId = 1;
            importsPath.CreatorUserId = 2;
            importsPath.CreationTime = DateTime.UtcNow;
           await _importPathsRepository.InsertAsync(importsPath);          

        }
    }
}
