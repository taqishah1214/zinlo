using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.ClosingChecklist.Dtos;
using Zinlo.Dto;

namespace Zinlo.ClosingChecklist.Exporting
{
    public interface ITaskExcelExporter
    {
        FileDto ExportToFile(List<TaskListDto> taskList);
    }
}
