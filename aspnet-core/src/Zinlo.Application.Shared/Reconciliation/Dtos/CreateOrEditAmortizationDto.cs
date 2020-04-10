using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Attachments.Dtos;
using Zinlo.Comment.Dtos;

namespace Zinlo.Reconciliation.Dtos
{
  public  class CreateOrEditAmortizationDto : CreationAuditedEntityDto<long>
    {
        public string InoviceNo { get; set; }
        public string JournalEntryNo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double Amount { get; set; }
        public double AccomulateAmount { get; set; }
        public string Description { get; set; }
        public long ChartsofAccountId { get; set; }
        public List<string> AttachmentsPath { get; set; }
        public List<GetAttachmentsDto> Attachments { get; set; }
        public Criteria Criteria { get; set; }
        public DateTime ClosingMonth { get; set; }
        public string CommentBody { get; set; }
        public List<CommentDto> Comments { get; set; }
        public bool IsDeleted { get; set; }



    }
    public enum Criteria
    {
        Manual = 1,
        Monthly = 2,
        Daily = 3,
    }
}
