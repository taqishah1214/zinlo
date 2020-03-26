using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ExceptionLogger.Dto
{
  public  class ExceptionLoggerForViewDto
    {
    public long Id { get; set; }
     public string Type { get; set; }
     //public int FailedRecordsCount { get; set; }
     //public int SuccessRecordsCount { get; set; }
     public string Records { get; set; }
     public string CreatedBy { get; set; }
     public string FilePath { get; set; }
     public DateTime CreationTime { get; set; }
    }
}
