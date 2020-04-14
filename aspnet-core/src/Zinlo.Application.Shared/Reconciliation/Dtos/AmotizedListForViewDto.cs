using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Attachments.Dtos;

namespace Zinlo.Reconciliation.Dtos
{
   public class AmortizedListForViewDto
    {
        public long Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public double  BeginningAmount { get; set; }
        public double AccuredAmortization { get; set; }
        public double  NetAmount { get; set; }
        public List<GetAttachmentsDto> Attachments { get; set; }
        public bool IsDeleted { get; set; }





    }
}
