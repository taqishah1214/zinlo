using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ClosingChecklist.Dtos
{
  public  class ChangeAssigneeDto
    {
        public long TaskId { get; set; }
        public long AssigneeId { get; set; }
       
    }
}
