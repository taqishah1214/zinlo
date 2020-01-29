using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Attachments.Dtos
{
   public class PostAttachmentsDto
    {
        public AttachmentType Type { get; set; }
        public long TypeId { get; set; }
        public IFormFile File { get; set; }
    }

    public enum AttachmentType
    {
        ClosingChecklist = 1,
    }
}
