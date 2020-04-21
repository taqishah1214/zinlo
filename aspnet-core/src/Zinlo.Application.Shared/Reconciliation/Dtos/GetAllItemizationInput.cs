using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Reconciliation.Dtos
{
  public   class GetAllItemizationInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public long ChartofAccountId { get; set; }
        public DateTime SelectedMonth { get; set; }
        public bool? AllOrActive { get; set; }

    }
}
