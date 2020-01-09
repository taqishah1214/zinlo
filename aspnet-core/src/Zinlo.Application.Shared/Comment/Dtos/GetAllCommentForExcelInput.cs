using System;
using System.Collections.Generic;
using System.Text;

namespace Zinlo.Comment.Dtos
{
   public class GetAllCommentForExcelInput
    {
        public string Filter { get; set; }

        public string DescriptionFilter { get; set; }

        public string TitleFilter { get; set; }
    }
}
