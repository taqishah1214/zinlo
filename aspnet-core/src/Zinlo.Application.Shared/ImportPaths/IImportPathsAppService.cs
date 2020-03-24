using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Zinlo.ImportPaths
{
  public  interface IImportPathsAppService
    {
        Task SaveFilePath(string url);
    }
}
