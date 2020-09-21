using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Abp.Timing;

namespace Zinlo.ClosingChecklist.Dtos
{
   public class GetClosingCheckListTaskDto : PagedAndSortedResultRequestDto
    { 
        public  ClosingCheckListForViewDto ClosingCheckListForViewDto { get; set; }
    }

    public class TasksGroup : GetClosingCheckListTaskDto
    {
        

        public List<GetUserWithPicture> OverallMonthlyAssignee { get; set; }
        [DisableDateTimeNormalization]
        public DateTime DueDate { get; set; }
        public IEnumerable<ClosingCheckListForViewDto> Group { get; set; }
        public bool MonthStatus { get; set; }
    }
}

