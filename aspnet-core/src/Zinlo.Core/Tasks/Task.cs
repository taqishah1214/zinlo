using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Authorization.Users;
using Zinlo.Categories;

namespace Zinlo.Tasks
{
  public  class Task : FullAuditedEntity, IMustHaveTenant
    {
        public virtual string TaskName { get; set; }
        public Category Category { get; set; }
        public virtual int CategoryId { get; set; }
        public User User { get; set; }
        public virtual int UserId { get; set; }
        public virtual DateTime? ClosingMonth { get; set; }
        public Comment.Comment Comment { get; set; }
        public virtual int CommentId { get; set; }
        public Status Status { get; set; }
        public int TenantId { get; set ; }
        public string FilePath { get; set; }
    }
    public enum Status
    {
        Complete =1,
        InProgress =2
    }
}
