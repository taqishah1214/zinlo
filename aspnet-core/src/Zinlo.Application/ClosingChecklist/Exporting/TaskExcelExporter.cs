using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Zinlo.ClosingChecklist.Dtos;
using Zinlo.DataExporting.Excel.EpPlus;
using Zinlo.Dto;
using Zinlo.Storage;

namespace Zinlo.ClosingChecklist.Exporting
{
    public class TaskExcelExporter : EpPlusExcelExporterBase, ITaskExcelExporter
    {
        public TaskExcelExporter(ITempFileCacheManager tempFileCacheManager)
            : base(tempFileCacheManager)
        {
        }

        public FileDto ExportToFile(List<TaskListDto> taskList)
        {
            return CreateExcelPackage(
                "Task List.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Tasks"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Task Name"),
                        L("Due Date"),
                        L("Closing Month"),
                        L("Status"),
                        L("Category"),
                        L("Assignee")
                    );

                    AddObjects(
                        sheet, 2, taskList,
                        _ => _.TaskName,
                        _ => _.DueDate.ToString("D", CultureInfo.InvariantCulture),
                        _ => _.ClosingMonth.ToString("Y",CultureInfo.InvariantCulture),
                        _ => _.Status,
                        _ => _.CategoryName,
                        _ => _.AssigneeName
                    );

                    //Formatting cells

                    var creationTimeColumn = sheet.Column(6);
                    creationTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";

                    for (var i = 1; i <= 6; i++)
                    {
                        sheet.Column(i).AutoFit();
                    }
                });
        }
    }
}
