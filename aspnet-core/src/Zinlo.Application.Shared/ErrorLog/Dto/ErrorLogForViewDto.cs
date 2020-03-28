using System;

namespace Zinlo.ErrorLog.Dto
{
    public class ErrorLogForViewDto
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Records { get; set; }
        public string CreatedBy { get; set; }
        public string FilePath { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
