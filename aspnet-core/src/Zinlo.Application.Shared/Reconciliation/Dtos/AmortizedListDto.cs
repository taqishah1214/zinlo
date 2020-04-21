using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Comment.Dtos;

namespace Zinlo.Reconciliation.Dtos
{
  public class AmortizedListDto 
    {
       public List<AmortizedListForViewDto> amortizedListForViewDtos { get; set; }
        public double TotalBeginningAmount { get; set; }
        public double TotalAccuredAmortization { get; set; }
        public double TotalNetAmount { get; set; }
        public double TotalTrialBalance { get; set; }
        public double VarianceNetAmount { get; set; }
        public double VarianceBeginningAmount { get; set; }
        public double VarianceAccuredAmount { get; set; }
        public List<CommentDto> Comments { get; set; }
        public int ReconciliedBase { get; set; }
        public bool MonthStatus { get; set; }


    }
}
