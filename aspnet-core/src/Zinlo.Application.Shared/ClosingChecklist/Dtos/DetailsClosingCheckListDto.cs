using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.Tasks.Dtos;

namespace Zinlo.ClosingChecklist.Dtos
{
  public  class DetailsClosingCheckListDto : CreateOrEditClosingChecklistDto
    {
        public string AssigneeName { get; set; }
    }
}
