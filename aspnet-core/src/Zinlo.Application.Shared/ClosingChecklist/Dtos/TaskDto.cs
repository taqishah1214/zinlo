using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Tasks.Dtos
{
  public  class TaskDto : EntityDto
    {

        public string TaskName { get; set; }
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public DateTime? ClosingMonth { get; set; }
        public int CommentId { get; set; }
        public int TenantId { get; set; }
        public string FilePath { get; set; }

    }
}
