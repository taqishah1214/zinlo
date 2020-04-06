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
        public long SaveFilePath(ImportPathDto input)
        {
            ImportsPath importsPath = new ImportsPath();
            importsPath.FilePath = input.FilePath;
            importsPath.SuccessFilePath = input.SuccessFilePath;
            importsPath.TenantId = input.TenantId;
            importsPath.Type = input.Type;
            importsPath.FailedRecordsCount = input.FailedRecordsCount;
            importsPath.SuccessRecordsCount = input.SuccessRecordsCount;
            importsPath.CreatorUserId = input.CreatorId;
            importsPath.UserId = input.CreatorId;
            importsPath.IsRollBacked = false;
            importsPath.CreationTime = DateTime.UtcNow;
          return  _importPathsRepository.InsertAndGetId(importsPath);          

        }

        public async Task UpdateFilePath(ImportPathDto input)
        {
            var output =  _importPathsRepository.FirstOrDefault(input.Id);
            if(output != null)
            {
                output.FilePath = input.FilePath;
                output.FailedRecordsCount = input.FailedRecordsCount;
                output.SuccessRecordsCount = input.SuccessRecordsCount;             
                await _importPathsRepository.UpdateAsync(output);

            }

        }
    }
}