using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ClosingChecklist.Dtos
{
   public class GetClosingCheckListTaskDto : PagedAndSortedResultRequestDto
    {
        public ClosingCheckListForViewDto ClosingCheckListForViewDto { get; set; }



    }
}
