using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zinlo.Comment.Dtos;
using System.Linq;
using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Zinlo.Comment
{
    public class CommentAppService : ZinloAppServiceBase, ICommentAppService
    {
        private readonly IRepository<Comment> _commentRepository;

        public CommentAppService(IRepository<Comment> commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public async Task Create(CreateOrEditCommentDto request)
        {
            var comment = ObjectMapper.Map<Comment>(request);
            await _commentRepository.InsertAsync(comment);
        }


        public async Task<List<CommentDto>> GetComments(int type, long typeId)
        {
            List<CommentDto> commentList = new List<CommentDto>();
            var taskComments = await _commentRepository.GetAll().Where(x => x.Type== (CommentType)type && x.TypeId == typeId).ToListAsync();
            if (taskComments.Count > 0)
            {
                foreach (var comment in taskComments)
                {
                    var commentObj = ObjectMapper.Map<CommentDto>(comment);
                    commentList.Add(commentObj);
                }
                return commentList;
            }
            else
            {
                return new List<CommentDto>();
            }


        }

    }
}
