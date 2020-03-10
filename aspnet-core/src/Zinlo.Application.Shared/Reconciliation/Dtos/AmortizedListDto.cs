using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Reconciliation.Dtos
{
  public class AmortizedListDto 
    {
       public List<AmortizedListForViewDto> amortizedListForViewDtos { get; set; }
        public double TotalBeginningAmount { get; set; }
        public double TotalAccuredAmortization { get; set; }
        public double TotalNetAmount { get; set; }
    }
}
