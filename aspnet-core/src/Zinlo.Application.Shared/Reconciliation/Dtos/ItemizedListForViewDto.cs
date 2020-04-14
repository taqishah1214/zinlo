using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Attachments.Dtos;

namespace Zinlo.Reconciliation.Dtos
{
   public class ItemizedListForViewDto
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public List<GetAttachmentsDto> Attachments { get; set; }
        public bool IsDeleted { get; set; }

    }
}
