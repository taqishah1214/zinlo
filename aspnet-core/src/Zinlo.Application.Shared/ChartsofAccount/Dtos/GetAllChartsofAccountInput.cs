using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ChartsofAccount.Dtos
{
   public class GetAllChartsofAccountInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public int AccountType { get; set; }
        public DateTime SelectedMonth { get; set; }
        public long AssigneeId { get; set; }
        public bool? AllOrActive { get; set; }
        public bool BeginingAmountCheck { get; set; }
        public int ReconciliationType { get; set; }
        public bool IncludeNotReconciled { get; set; }
        


    }
}
