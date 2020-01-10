using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Comment.Dtos
{
  public  class GetCommentForViewDto
    {
        public CommentDto Comment { get; set; }
        public string Type { get; set; }
        public int TypeId { get; set; }
        public string Body { get; set; }
    }
}
