using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;

namespace Zinlo.Versions.Dto
{
    public class CreateOrEditVersion : EntityDto<long>
    {
        public string Body { get; set; }
        public bool Active { get; set; }
        public Type Type { get; set; }
        public long TypeId { get; set; }

    }

    public class GetVersion : EntityDto<long>
    {
        public string Body { get; set; }
    }
    public enum TypeDto
    {
        ClosingChecklist = 1,
        ChartOfAccount = 2,
    }
}
