using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.ImportPaths.Dto;

namespace Zinlo.ImportPaths
{
  public  interface IImportPathsAppService
    {
        long SaveFilePath(ImportPathDto input);
        Task UpdateFilePath(ImportPathDto input);
    }
}
