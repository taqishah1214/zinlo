using Abp;
using System;

namespace Zinlo.Reconciliation.Importing
{
    public class ImportItemizedFromExcelJobArgs
    {
        public int? TenantId { get; set; }
        public Guid BinaryObjectId { get; set; }
        public UserIdentifier User { get; set; }
        public long ChartsofAccountsId { get; set; }
        public DateTime SelectedMonth { get; set; }
        public string Url { get; set; }
    }
}
