using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Comment.Dtos
{
  public  class CreateOrEditCommentDto : EntityDto<int?>
    {
        public string Type { get; set; }
        public int TypeId { get; set; }
        public string Body { get; set; }
    }
}
