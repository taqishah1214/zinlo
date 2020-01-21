using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ClosingChecklist.Dtos
{
  public   class ClosingCheckListForViewDto
    {
        public string TaskName { get; set; }

        public string Category { get; set; }

        public string Status { get; set; }

        public string AssigniName { get; set; }
        public long Id { get; set; }
        public long StatusId { get; set; }
        public long AssigneeId { get; set; }
    }

   
}
