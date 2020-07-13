using Abp.Domain.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

       
        public async Task<List<string>> PostAttachmentFile()
        {
            var file= (IFormFile)null;
            List<string> finalFilePath = new List<string>();
            var ctx = _httpContextAccessor.HttpContext.Request.Form.Files;
            if (!Directory.Exists(_env.WebRootPath + "/Uploads-Attachments"))
            {
                Directory.CreateDirectory(_env.WebRootPath + "/Uploads-Attachments");
            }
            foreach(var item in ctx)
            {
               file =   item;
                var OriginalFileName =file.FileName.Substring(0, file.FileName.LastIndexOf("."));
                string uniqueFile = OriginalFileName + "zinlo"+Guid.NewGuid() + "" + file.FileName.Substring(file.FileName.LastIndexOf(".", StringComparison.Ordinal));
                var path = Path.Combine(Path.Combine(_env.WebRootPath, "Uploads-Attachments"), uniqueFile);
                using (var fc = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fc);
                }
                var filePath = "Uploads-Attachments/" + uniqueFile;
                finalFilePath.Add(filePath);
            }
           
            return finalFilePath;    
        }


        public async Task PostAttachmentsPath(PostAttachmentsPathDto input)
        {    
            foreach (var item in input.FilePath)
            {
                Attachment.Attachment attachment = new Attachment.Attachment();
                attachment.FilePath = item;
                attachment.TypeId = input.TypeId;
                attachment.Type =(AttachmentType)input.Type;
               await _attachment.InsertAsync(attachment);
            }           
        }

        public async Task<List<GetAttachmentsDto>> GetAttachmentsPath(long typeId,long type) 
        {
           List<GetAttachmentsDto> attachments = new List<GetAttachmentsDto>();
           var attachmentResult = await _attachment.GetAll().Where(x => x.Type == (AttachmentType)type && x.TypeId == typeId).ToListAsync();

            if (attachmentResult != null)
            {
              foreach(var item in attachmentResult)
                {
                    GetAttachmentsDto getAttachmentsDto = new GetAttachmentsDto();
                    getAttachmentsDto.attachmentPath = item.FilePath.ToString();
                    getAttachmentsDto.id = item.Id;
                    attachments.Add(getAttachmentsDto);
                }

            }
            return attachments;

        }

        public async Task DeleteAttachmentPath(long id)
        {
           await _attachment.DeleteAsync(id);
            
        }

        public List<string> GetAttachmentPathById(long typeId, long type)
        {
            var attachments =  _attachment.GetAll().Where(x => x.Type == (AttachmentType)type && x.TypeId == typeId).Select(p=>p.FilePath).ToList();
            return attachments;

        }
    }
}
