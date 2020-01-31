using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Comment.Dtos;

namespace Zinlo.ClosingChecklist.Dtos
{
    public class GetTaskForEditDto
    {
        public long Id { get; set; }
        public  string TaskName { get; set; }
        public DateTime ClosingMonth { get; set; }
        public string Category { get; set; }
        public string AssigniName { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public string Attachment { get; set; }
        public string Instruction { get; set; }
        public int NoOfMonths { get; set; }
        public int DueOn { get; set; }
        public DateTime EndsOn { get; set; }
        public bool DayBeforeAfter { get; set; }
        public bool EndOfMonth { get; set; }
        public string Frequency { get; set; }
        public long CategoryId { get; set; }
        public long AssigneeId { get; set; }
        public int FrequencyId { get; set; }
        public List<CommentDto> comments { get; set; }

    }
}
