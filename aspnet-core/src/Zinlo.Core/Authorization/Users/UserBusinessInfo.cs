using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Authorization.Users
{
    public class UserBusinessInfo: FullAuditedEntity<long>, IMustHaveTenant
    {
        public string BusinessName { get; set; }
        public string Website { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressLineOne { get; set; }
        public string AddressLineTwo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public int TenantId { get ; set ; }
    }
}
