using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Zinlo.Comment.Dtos;

namespace Zinlo.ClosingChecklist.Dtos
{
    public class CreateOrEditClosingChecklistDto : EntityDto<long>
    {
        public virtual string TaskName { get; set; }
        public virtual long CategoryId { get; set; }
        public virtual long AssigneeId { get; set; }
        public virtual DateTime ClosingMonth { get; set; }
        public StatusDto Status { get; set; }
        public string Instruction { get; set; }
        public int NoOfMonths { get; set; }
        public int DueOn { get; set; }
        public DateTime DueDate { get; set; }
        public DaysBeforeAfterDto DayBeforeAfter { get; set; }
        public bool EndOfMonth { get; set; }
        public FrequencyDto Frequency { get; set; }
        public string CommentBody { get; set; }
        public List<CommentDto> Comments { get; set; }
        public List<string> AttachmentsPath { get; set; }
        public Guid? GroupId { get; set; }
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
        XNumberOfMonths = 4,
        None = 5
    }
    public enum DaysBeforeAfterDto
    {
        None = 1,
        DaysBefore = 2,
        DaysAfter = 3
    }
}