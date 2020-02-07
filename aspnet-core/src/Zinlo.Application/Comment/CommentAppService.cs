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
using Zinlo.Authorization.Users;

namespace Zinlo.Comment
{
    public class CommentAppService : ZinloAppServiceBase, ICommentAppService
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<User, long> _userRepository;

        public CommentAppService(IRepository<Comment> commentRepository, IRepository<User, long> userRepository)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
        }

        public async Task Create(CreateOrEditCommentDto request)
        {
            var comment = ObjectMapper.Map<Comment>(request);
            await _commentRepository.InsertAsync(comment);
        }

        public async Task Update(CreateOrEditCommentDto request)
        {
            var comment = await _commentRepository.FirstOrDefaultAsync((int)request.Id);
            var data = ObjectMapper.Map(request, comment);
            await _commentRepository.UpdateAsync(data);
        }


        public async Task<List<CommentDto>> GetComments(int type, long typeId)
        {
            List<CommentDto> commentList = new List<CommentDto>();
            var taskComments = await _commentRepository.GetAll().Where(x => x.Type== (CommentType)type && x.TypeId == typeId).ToListAsync();

            if (taskComments.Count > 0)
            {
                foreach (var comment in taskComments)
                {
                    CommentDto commentDto = new CommentDto();
                    commentDto.Id = comment.Id;
                    commentDto.Type = comment.Type.ToString();
                    commentDto.TypeId = (int)comment.TypeId;
                    commentDto.UserName = _userRepository.FirstOrDefaultAsync((long)comment.CreatorUserId).Result.FullName;
                    commentDto.Body = comment.Body;
                    commentDto.UserProfilePath = ""; // Will pass profile url from s3 in future.
                    commentDto.CreationDateTime = comment.CreationTime;
                    commentDto.DaysCount = CalculateDays(comment.CreationTime);
                    commentList.Add(commentDto);
                }
                return commentList;
            }
            else
            {
                return new List<CommentDto>();
            }


        }
        public string CalculateDays(DateTime dateTime)
        {
            double COUNT = (DateTime.Now - dateTime).TotalDays;
            COUNT = Math.Ceiling(COUNT);

            return COUNT + " "+"days ago".ToString();
        }


    }
}
