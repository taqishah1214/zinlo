using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using Abp.Auditing;
using Zinlo.Authorization.Users;
using Zinlo.Categories;

namespace Zinlo.ClosingChecklist
{ 
    [Audited]
    public class ClosingChecklist : FullAuditedEntity<long>, IMustHaveTenant
    {
        public virtual string TaskName { get; set; }
        public Category Category { get; set; }
        public virtual long CategoryId { get; set; }
        public User Assignee { get; set; }
        public virtual long AssigneeId { get; set; }
        public virtual DateTime ClosingMonth { get; set; }
        public Status Status { get; set; }
        [DisableAuditing]
        public int TenantId { get; set; }
        public string Instruction { get; set; }
        public int NoOfMonths { get; set; }
        [DisableAuditing]
        public int DueOn { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? EndsOn { get; set; }
        public DaysBeforeAfter DayBeforeAfter { get; set; }
        public bool EndOfMonth { get; set; }
        public Frequency Frequency { get; set; }
        [DisableAuditing]
        public Guid GroupId { get; set; }
    }
    public enum Status
    {
        NotStarted = 1,
        InProcess = 2,
        OnHold = 3,
        Completed = 4
    }
    public enum Frequency
    {         
        Monthly = 1,
        Quarterly = 2,
        Annually = 3,
        XNumberOfMonths = 4,
        None = 5
    }
    public enum DaysBeforeAfter
    {
        None = 1,
        DaysBefore = 2,
        DaysAfter = 3
    }
}
