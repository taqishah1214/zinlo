using Abp.Application.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zinlo.Comment.Dtos;

namespace Zinlo.Comment
{
    public interface ICommentAppService : IApplicationService
    {
        Task Create(CreateOrEditCommentDto request);
        Task Update(CreateOrEditCommentDto request);
        Task<List<CommentDto>> GetComments(int type,long typeId);
    }
}
