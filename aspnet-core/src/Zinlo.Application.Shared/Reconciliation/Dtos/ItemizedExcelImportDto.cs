using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Attachments.Dtos;
using Zinlo.Comment.Dtos;

namespace Zinlo.Reconciliation.Dtos
{
    public class ItemizedExcelImportDto
    {
        public string InoviceNo { get; set; }
        public string JournalEntryNo { get; set; }
        public string Date { get; set; }
        public string Amount { get; set; }
        public string Description { get; set; }
        public string Exception { get; set; }
        public bool CanBeImported()
        {
            return string.IsNullOrEmpty(Exception);
        }
        public bool isValid { get; set; }

    }
}
