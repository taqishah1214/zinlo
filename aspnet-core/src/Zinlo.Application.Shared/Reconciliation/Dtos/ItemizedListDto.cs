using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Reconciliation.Dtos
{
   public class ItemizedListDto
    {
        public List<ItemizedListForViewDto> itemizedListForViewDto { get; set; }
        public double TotalAmount { get; set; }
        public double TotalTrialBalance { get; set; }
        public double Variance { get; set; }

    }
}
