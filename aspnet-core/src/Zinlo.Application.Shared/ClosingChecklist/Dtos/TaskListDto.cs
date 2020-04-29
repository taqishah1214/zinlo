using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ClosingChecklist.Dtos
{
    public class TaskListDto
    {
        public string TaskName { get; set; }
        public string CategoryName { get; set; }
        public string AssigneeName { get; set; }
        public string Status { get; set; }
        public DateTime DueDate { get; set; }
        public virtual DateTime ClosingMonth { get; set; }
    }
}
