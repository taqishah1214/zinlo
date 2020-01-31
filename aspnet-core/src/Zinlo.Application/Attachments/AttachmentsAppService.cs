using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Attachment;
using Zinlo.Attachments.Dtos;

namespace Zinlo.Attachments
{
    public class AttachmentsAppService : ZinloAppServiceBase, IAttachmentAppService
    {
        private readonly IRepository<Attachment.Attachment, long> _attachment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _env;

        public AttachmentsAppService(IRepository<Attachment.Attachment, long> attachment, IHostingEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _attachment = attachment;
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }

       
        public async Task<string> PostAttachmentFile()
        {
            var file= (IFormFile)null;
            var ctx = _httpContextAccessor.HttpContext.Request.Form.Files;
            foreach(var item in ctx)
            {
               file =   item;
            }
            var path = Path.Combine(Path.Combine(_env.WebRootPath, "Uploads-Attachments"), file.FileName);
            using (var fc = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fc);
            }

            var filePath = "http://localhost:22742/Uploads-Attachments/" + file.FileName;
            return filePath;    
        }

        public async Task PostAttachmentsPath(PostAttachmentsPathDto input)
        {
            Attachment.Attachment attachment = new Attachment.Attachment();
            foreach (var item in input.FilePath)
            {
                attachment.FilePath = item;
                attachment.TypeId = input.TypeId;
                attachment.Type =(AttachmentType)input.Type;
               await _attachment.InsertAsync(attachment);

            }

           
        }
    }
}
