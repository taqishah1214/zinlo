using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ImportPaths.Dto
{
  public  class ImportPathDto
    {
        public string FilePath { get; set; }
        public string Type { get; set; }
        public int FailedRecordsCount { get; set; }
        public int SuccessRecordsCount { get; set; }
        public int TenantId { get; set; }
        public long CreatorId { get; set;}
    }
}
