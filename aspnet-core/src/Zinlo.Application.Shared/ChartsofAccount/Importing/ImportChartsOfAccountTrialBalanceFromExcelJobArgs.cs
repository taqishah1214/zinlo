using Abp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ChartsofAccount.Importing
{
    public class ImportChartsOfAccountTrialBalanceFromExcelJobArgs
    {
        public int? TenantId { get; set; }
        public Guid BinaryObjectId { get; set; }
        public UserIdentifier User { get; set; }
        public DateTime selectedMonth { get; set; }

    }
}
