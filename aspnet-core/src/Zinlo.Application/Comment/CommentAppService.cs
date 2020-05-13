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
            var commentList = new List<CommentDto>();
            var taskComments = await _commentRepository.GetAll().Where(x => x.Type == (CommentType)type && x.TypeId == typeId).ToListAsync();
            if (taskComments.Count <= 0) return new List<CommentDto>();
            commentList.AddRange(taskComments.Select(comment => new CommentDto
            {
                Id = comment.Id,
                Type = comment.Type.ToString(),
                TypeId = (int) comment.TypeId,
                UserName = _userRepository.FirstOrDefaultAsync((long) comment.CreatorUserId).Result.FullName,
                Body = comment.Body,
                ProfilePicture = "",
                CreationTime = comment.CreationTime,
                DaysCount = CalculateDays(comment.CreationTime)
            }));
            return commentList.OrderByDescending(p=>p.CreationTime).ToList();

        }
        public string CalculateDays(DateTime dateTime)
        {
            double COUNT = (DateTime.Now - dateTime).TotalDays;
            COUNT = Math.Ceiling(COUNT);
            return COUNT + " " + "days ago".ToString();
        }


    }
}
