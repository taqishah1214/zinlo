using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Comment.Dtos;

namespace Zinlo.ClosingChecklist.Dtos
{
    public class GetTaskForEditDto
    {
        public virtual string TaskName { get; set; }
        public virtual DateTime ClosingMonth { get; set; }

        public string Category { get; set; }
        public string AssigniName { get; set; }
        public string Status { get; set; }
        public string Attachment { get; set; }
        public string Instruction { get; set; }
        public int NoOfMonths { get; set; }
        public int DueOn { get; set; }
        public DateTime EndsOn { get; set; }
        public bool DayBeforeAfter { get; set; }
        public bool EndOfMonth { get; set; }
        public string Frequency { get; set; }
        public List<CommentDto> comments { get; set; }

    }
}
