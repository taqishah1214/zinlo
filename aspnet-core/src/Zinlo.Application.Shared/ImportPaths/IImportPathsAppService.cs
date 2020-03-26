using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.ImportPaths.Dto;

namespace Zinlo.ImportPaths
{
  public  interface IImportPathsAppService
    {
        Task SaveFilePath(ImportPathDto input);
    }
}
