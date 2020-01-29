using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Attachments.Dtos
{
    public class AttachmentsDto: Entity<long>
    {
        public long TypeId { get; set; }
        public string FilePath { get; set; }
    }
}
