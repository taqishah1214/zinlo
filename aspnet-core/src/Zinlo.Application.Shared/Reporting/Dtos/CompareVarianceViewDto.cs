using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Reporting.Dtos
{
    public class CompareVarianceViewDto
    {
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public double FirstMonthVariance { get; set; }
        public double SecondMonthVariance { get; set; }
    }
}
