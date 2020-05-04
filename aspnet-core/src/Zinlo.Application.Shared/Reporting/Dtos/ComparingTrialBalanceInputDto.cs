using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Reporting.Dtos
{
   public class ComparingTrialBalanceInputDto : PagedAndSortedResultRequestDto
    {
        public int FirstMonthId { get; set; }
        public int SecondMonthId { get; set; }

    }
}
