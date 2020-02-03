using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using Zinlo.Authorization.Users;
using Zinlo.Categories;

namespace Zinlo.ClosingChecklist
{
    public class ClosingChecklist : FullAuditedEntity<long>, IMustHaveTenant
    {
        public virtual string TaskName { get; set; }
        public Category Category { get; set; }
        public virtual long CategoryId { get; set; }
        public User Assignee { get; set; }
        public virtual long AssigneeId { get; set; }
        public virtual DateTime ClosingMonth { get; set; }
        public Status Status { get; set; }
        public int TenantId { get; set; }
        public string Instruction { get; set; }
        public int NoOfMonths { get; set; }
        public int DueOn { get; set; }
        public DateTime EndsOn { get; set; }
        public bool DayBeforeAfter { get; set; }
        public bool EndOfMonth { get; set; }
        public Frequency Frequency { get; set; }
    }
    public enum Status
    {
        Open = 1,
        Complete = 2,
        Inprogress = 3
    }
    public enum Frequency
    {
        Monthly = 1,
        Quarterly = 2,
        Annually = 3,
        XNumberOfMonths = 4
    }
}
