using System;
using System.Collections.Generic;
using System.Text;
using Abp.Timing;

namespace Zinlo.ClosingChecklist.Dtos
{
    [DisableDateTimeNormalization]
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
