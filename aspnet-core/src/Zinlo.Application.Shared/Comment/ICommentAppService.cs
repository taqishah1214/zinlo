using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Comment.Dtos;

namespace Zinlo.Comment
{
    public interface ICommentAppService : IApplicationService
    {
        Task Create(CreateOrEditCommentDto request);
    }
}
