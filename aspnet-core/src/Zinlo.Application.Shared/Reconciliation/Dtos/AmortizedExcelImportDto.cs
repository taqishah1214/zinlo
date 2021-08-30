using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Reconciliation.Dtos
{
    public class AmortizedExcelImportDto
    {
        public string InvoiceNo { get; set; }
        public string JournalEntryNo { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string Criteria { get; set; }
        public bool IsValid { get; set; }
        public string Exception { get; set; }
        public bool CanBeImported()
        {
            return string.IsNullOrEmpty(Exception);
        }

    }
}
