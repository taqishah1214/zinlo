using Abp.Application.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Attachments.Dtos;

namespace Zinlo.Attachments
{
    public interface IAttachmentAppService : IApplicationService
    {
        Task<List<string>> PostAttachmentFile();

        Task PostAttachmentsPath(PostAttachmentsPathDto input);

        Task<List<GetAttachmentsDto>> GetAttachmentsPath(long typeId, long type);

        Task DeleteAttachmentPath(long id);



    }
}
