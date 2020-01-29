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
        Task PostAttachment(List<PostAttachmentsDto> input);

    }
}
