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
        public int TenantId { get; set; }
        public string Instruction { get; set; }
        public int NoOfMonths { get; set; }
        public int DueOn { get; set; }
        public DateTime EndsOn { get; set; }
        public bool DayBeforeAfter { get; set; }
        public bool EndOfMonth { get; set; }
        public FrequencyDto Frequency { get; set; }
        public string CommentBody { get; set; }
        public List<CommentDto> comments { get; set; }
    }
    public enum StatusDto
    {
        Open = 1,
        Complete = 2,
        Inprogress = 3
    }
    public enum FrequencyDto
    {
        Monthly = 1,
        Quarterly = 2,
        Annually = 3,
        XNumberOfMonths = 4
    }
}