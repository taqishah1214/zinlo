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
        ItemizedItem = 2,
        AmortizedItem = 3,
        ItemizedList = 4,
        AmortizedList = 5,
    }
}
