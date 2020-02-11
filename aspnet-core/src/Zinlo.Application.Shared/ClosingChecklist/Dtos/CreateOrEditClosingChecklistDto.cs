using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Attachments.Dtos;
using Zinlo.Comment.Dtos;

namespace Zinlo.Tasks.Dtos
{
    public class CreateOrEditClosingChecklistDto : CreationAuditedEntityDto<long>
    {
        public virtual string TaskName { get; set; }
        public virtual long CategoryId { get; set; }
        public virtual long AssigneeId { get; set; }
        public virtual DateTime ClosingMonth { get; set; }
        public StatusDto Status { get; set; }
        public string Instruction { get; set; }
        public int NoOfMonths { get; set; }
        public int DueOn { get; set; }
        public DateTime EndsOn { get; set; }
        public bool DayBeforeAfter { get; set; }
        public bool EndOfMonth { get; set; }
        public FrequencyDto Frequency { get; set; }
        public string CommentBody { get; set; }
        public List<CommentDto> comments { get; set; }
        public List<string> AttachmentsPath { get; set; }
    }
    public enum StatusDto
    {
        NotStarted = 1,
        InProcess = 2,
        OnHold = 3,
        Completed = 4
    }
    public enum FrequencyDto
    {
        Monthly = 1,
        Quarterly = 2,
        Annually = 3,
        XNumberOfMonths = 4
    }
}