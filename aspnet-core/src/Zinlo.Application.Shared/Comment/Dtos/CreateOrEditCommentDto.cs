using Abp.Application.Services.Dto;

namespace Zinlo.Comment.Dtos
{
    public  class CreateOrEditCommentDto : CreationAuditedEntityDto<long>
    {
        public CommentTypeDto Type { get; set; }
        public long TypeId { get; set; }
        public string Body { get; set; }
    }
    public enum CommentTypeDto
    {
        ClosingChecklist =1,
    }
}
