using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Register_User.Dto
{
    public class PersonalInfoDto
    {
        public string UserName { get; set; }
        public string Title { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }

    }
}
