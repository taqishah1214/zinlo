using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Contactus.Dto
{
    public class CreateOrUpdateContactusInput
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public int NumberOfUsers { get; set; }
        public string Description { get; set; }
    }
}
