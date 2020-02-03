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
    }
}
