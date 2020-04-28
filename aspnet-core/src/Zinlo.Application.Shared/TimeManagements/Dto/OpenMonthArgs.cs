using System;
using System.Collections.Generic;
using System.Text;
using Abp;

namespace Zinlo.TimeManagements.Dto
{
    [Serializable]
    public class OpenMonthArgs
    {
        public DateTime Month { get; set; }
        public UserIdentifier UserIdentifier { get; set; }
    }

}
