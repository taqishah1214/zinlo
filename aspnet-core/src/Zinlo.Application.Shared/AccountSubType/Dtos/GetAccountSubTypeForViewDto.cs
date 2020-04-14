using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.AccountSubType.Dtos
{
    public class GetAccountSubTypeForViewDto : EntityDto<long>
    {
        public string Title { get; set; }

        public string Description { get; set; }
        public DateTime CreationDate { get; set; }
        public long? UserId { get; set; }
    }
}
