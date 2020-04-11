using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Attachments.Dtos;
using Zinlo.Comment.Dtos;
using Zinlo.Tasks.Dtos;

namespace Zinlo.ClosingChecklist.Dtos
{
    public class GetTaskForEditDto
    {
        public long Id { get; set; }
        public int TenantId { get; set; }
        public  string TaskName { get; set; }
        public DateTime ClosingMonth { get; set; }
        public string Category { get; set; }
        public string AssigniName { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public List<GetAttachmentsDto> Attachments { get; set; }
        public string InstructionBody { get; set; }
        public int NoOfMonths { get; set; }
        public int DueOn { get; set; }
        public DateTime EndsOn { get; set; }
        public DateTime DueDate { get; set; }
        public DaysBeforeAfterDto DayBeforeAfter { get; set; }
        public bool EndOfMonth { get; set; }
        public bool MonthStatus { get; set; }
        public long CategoryId { get; set; }
        public long AssigneeId { get; set; }
        public int FrequencyId { get; set; }
        public List<CommentDto> Comments { get; set; }
        public string ProfilePicture { get; set; }
        public Guid GroupId { get; set; }
    }
}
