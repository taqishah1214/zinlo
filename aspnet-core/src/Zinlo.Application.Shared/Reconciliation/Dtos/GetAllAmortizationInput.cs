﻿using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Reconciliation.Dtos
{
    public class GetAllAmortizationInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public long ChartofAccountId { get; set; }

        public string MonthFilter { get; set; }

    }
}
