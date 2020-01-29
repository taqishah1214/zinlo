using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Attachments.Dtos;

namespace Zinlo.Attachments
{
    public class AttachmentsAppService : ZinloAppServiceBase, IAttachmentAppService
    {
        private readonly IRepository<Attachment.Attachment, long> _attachment;
        public AttachmentsAppService(IRepository<Attachment.Attachment, long> attachment)
        {
            _attachment = attachment;
        }

        public async Task PostAttachment([FromForm] List<PostAttachmentsDto> input)
        {
            foreach (var item in input)
            {
                using (FileStream fileStream = System.IO.File.Create("Upload\\" + item.File.FileName))
                {
                    item.File.CopyTo(fileStream);
                    fileStream.Flush();
                }
                AttachmentsDto attachmentsDto = new AttachmentsDto();
                attachmentsDto.TypeId = item.TypeId;
                attachmentsDto.FilePath = "Uploads\\" + item.File.FileName.ToString();
                var result = ObjectMapper.Map<Attachment.Attachment>(attachmentsDto);

                await _attachment.InsertAsync(result);

            }
        }
    }
}
