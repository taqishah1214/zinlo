using Abp;
using System;

namespace Zinlo.ClosingChecklist.Importing
{
    public class ImportClosingChecklistFromExcelJobArgs
    {
        public int? TenantId { get; set; }
        public Guid BinaryObjectId { get; set; }
        public UserIdentifier User { get; set; }
        public DateTime selectedMonth { get; set; }
        public string url { get; set; }
    }
}
