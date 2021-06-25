using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.SystemSettings.Dtos
{
   public class CreateOrEditDefaultMonthDto
    {
        public long Id { get; set; }
        public bool IsWeekEndEnable { get; set; }
        public DateTime Month { get; set; }
    }
}
