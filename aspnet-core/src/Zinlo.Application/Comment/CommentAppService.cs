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
        private readonly IRepository<Comment> _commentRepostory;

        public CommentAppService(IRepository<Comment> commentRepository)
        {
            _commentRepostory = commentRepository;
        }

		public async Task Create(CreateOrEditCommentDto request)
		{
			var comment = ObjectMapper.Map<Comment>(request);
			await _commentRepostory.InsertAsync(comment);
		}

		//      public async Task<PagedResultDto<GetCommentForViewDto>> GetAll(GetAllCommentInput input)
		//      {
		//		 var filteredComments = _commentRepostiry.GetAll()
		//				.WhereIf(!string.IsNullOrWhiteSpace(input.Filter), e => false || e.Type.Contains(input.Filter) || e.Type.Contains(input.Filter))
		//				.WhereIf(!string.IsNullOrWhiteSpace(input.DescriptionFilter), e => e.Type == input.DescriptionFilter)
		//				.WhereIf(!string.IsNullOrWhiteSpace(input.TitleFilter), e => e.Type == input.TitleFilter);

		//	var pagedAndFilteredComments = filteredComments
		//		.OrderBy(input.Sorting ?? "id asc")
		//		.PageBy(input);

		//	var commments = from o in pagedAndFilteredComments
		//					 select new GetCommentForViewDto()
		//					 {
		//						Comment = new CommentDto
		//						{
		//							 Id = o.Id,
		//							 Type = o.Type,
		//							  TypeId = o.TypeId									  
		//						}
		//					 };

		//	var totalCount = await filteredComments.CountAsync();
		//	return new PagedResultDto<GetCommentForViewDto>(
		//		totalCount,
		//		await commments.ToListAsync()
		//	);
		//}


	}
}
