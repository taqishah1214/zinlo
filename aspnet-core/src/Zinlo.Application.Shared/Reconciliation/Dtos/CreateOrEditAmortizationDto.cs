﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
