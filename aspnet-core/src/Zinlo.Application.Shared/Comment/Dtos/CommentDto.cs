using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Comment.Dtos
{
  public  class CommentDto : EntityDto
    {
        public string Type { get; set; }
        public int TypeId { get; set; }
        public string Body { get; set; }
        public string UserName { get; set; }
        public DateTime? CreationDateTime { get; set; }
        public string ProfilePicture { get; set; }
        public string DaysCount { get; set; }
    }
}
