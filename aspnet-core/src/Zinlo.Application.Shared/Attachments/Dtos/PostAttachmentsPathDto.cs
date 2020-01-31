using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Attachments.Dtos
{
   public class PostAttachmentsPathDto 
    {
        public int Type { get; set; }
        public long TypeId { get; set; }
        public List<string> FilePath { get; set; }
    }
}
