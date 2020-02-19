﻿using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Attachments.Dtos;
using Zinlo.Comment.Dtos;
using Zinlo.Tasks.Dtos;

namespace Zinlo.ClosingChecklist.Dtos
{
  public  class DetailsClosingCheckListDto : CreateOrEditClosingChecklistDto
    {
        public string AssigneeName { get; set; }
        public string CategoryName { get; set; }
        public string TaskStatus { get; set; }

        public string ProfilePicture { get; set; }

        public  List<CommentDto>  comments { get; set; }

        public List<GetAttachmentsDto> Attachments { get; set; }
    }
    
}
