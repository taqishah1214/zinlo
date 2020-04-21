using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Contactus.Dto
{
    public class ContactusDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public int NumberOfUsers { get; set; }
        public string Description { get; set; }
        public int Commitment { get; set; }
        public decimal Pricing { get; set; }
        public int TenantId { get; set; }
        public bool IsAccepted { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
