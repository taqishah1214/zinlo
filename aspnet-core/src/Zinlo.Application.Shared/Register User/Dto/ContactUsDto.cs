using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Register_User.Dto
{
    public class ContactUsDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public int NumberOfUsers { get; set; }
        public string Description { get; set; }
    }
}
