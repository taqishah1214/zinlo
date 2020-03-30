using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Attachments.Dtos;

namespace Zinlo.Reconciliation.Dtos
{
  public  class CreateOrEditItemizationDto : CreationAuditedEntityDto<long>
    {
        public string InoviceNo { get; set; }
        public string JournalEntryNo { get; set; }
        public DateTime Date { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public long ChartsofAccountId { get; set; }
        public List<string> AttachmentsPath { get; set; }   
        public List<GetAttachmentsDto> Attachments { get; set; }
        public DateTime ClosingMonth { get; set; }



    }
}
