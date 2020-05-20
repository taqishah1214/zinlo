using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ImportPaths.Dto
{
    public class ImportPathDto
    {
        public long Id { get; set; }
        public string FilePath { get; set; }
        public string Type { get; set; }
        public int FailedRecordsCount { get; set; }
        public int SuccessRecordsCount { get; set; }
        public int TenantId { get; set; }
        public long CreatorId { get; set; }
        public string SuccessFilePath { get; set; }
        public string  UploadedFilePath { get; set; }
        public DateTime UploadMonth { get; set; }
        public Status FileStatus { get; set; }

    }
        public enum Status
        {
            InProcess = 1,
            Completed = 2
        }
}