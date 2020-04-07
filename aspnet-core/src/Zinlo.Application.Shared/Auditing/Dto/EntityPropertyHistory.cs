using System;
using Abp.Application.Services.Dto;

namespace Zinlo.Auditing.Dto
{
    public class EntityPropertyHistory : EntityDto<long>
    {
        public long UserId { get; set; }

        public string NewValue { get; set; }

        public string OriginalValue { get; set; }

        public string PropertyName { get; set; }
        public DateTime ChangeTime { get; set; }

    }
}