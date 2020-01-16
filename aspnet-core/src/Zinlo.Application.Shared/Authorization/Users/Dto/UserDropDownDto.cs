using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Authorization.Users.Dto
{
    class UserDropDownDto : EntityDto<long>
    {   
            public long Id { get; set; }
            public string Name { get; set; }
        
    }
}
