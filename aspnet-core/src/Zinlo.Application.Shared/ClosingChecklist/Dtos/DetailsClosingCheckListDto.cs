using System;
using System.Collections.Generic;
using Abp.Timing;
using Zinlo.Attachments.Dtos;
using Zinlo.Comment.Dtos;

namespace Zinlo.ClosingChecklist.Dtos
{
    public class DetailsClosingCheckListDto : CreateOrEditClosingChecklistDto
    {
        public string AssigneeName { get; set; }
        public string CategoryName { get; set; }
        public long CategoryId { get; set; }
        public string TaskStatus { get; set; }

        public string ProfilePicture { get; set; }
        public bool MonthStatus { get; set; }
        [DisableDateTimeNormalization]
        public DateTime DueDate { get; set; }
        public string InstructionBody { get; set; }
        public List<CommentDto> Comments { get; set; }
        public List<GetAttachmentsDto> Attachments { get; set; }
        public bool IsDeleted { get; set; }
    }

}
