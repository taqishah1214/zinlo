using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ClosingChecklist.Dtos
{
  public  class GetAllClosingCheckListInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public string TitleFilter { get; set; }
        public int CategoryFilter { get; set; }
        public int StatusFilter { get; set; }      
        public DateTime? DateFilter { get; set; }
        public string MonthFilter { get; set; }
    }
}
