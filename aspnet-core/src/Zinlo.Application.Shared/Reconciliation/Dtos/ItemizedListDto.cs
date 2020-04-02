using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Comment.Dtos;

namespace Zinlo.Reconciliation.Dtos
{
   public class ItemizedListDto
    {
        public List<ItemizedListForViewDto> itemizedListForViewDto { get; set; }
        public double TotalAmount { get; set; }
        public double TotalTrialBalance { get; set; }
        public double Variance { get; set; }
        public List<CommentDto> Comments { get; set; }



    }
}
