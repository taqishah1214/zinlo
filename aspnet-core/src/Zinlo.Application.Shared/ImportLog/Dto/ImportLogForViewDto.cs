using System;

namespace Zinlo.ImportLog.Dto
{
    public class ImportLogForViewDto
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string Records { get; set; }
        public string CreatedBy { get; set; }
        public string FilePath { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsRollBacked { get; set; }
        public string SuccessFilePath { get; set; }
    }
}
