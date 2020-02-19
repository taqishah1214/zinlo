using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.ChartsofAccount.Dtos
{
   public class GetAllChartsofAccountInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }
        public string TitleFilter { get; set; }
    }
}
