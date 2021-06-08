using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zinlo.ClosingChecklist.Dtos;
using Zinlo.ClosingChecklist.Importing;
using Zinlo.DataExporting.Excel.EpPlus;
using Zinlo.Storage;

namespace Zinlo.ClosingChecklist
{
    public class InvalidClosingChecklistExporter : EpPlusExcelExporterBase, IInvalidClosingChecklistExporter
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _env;
        public InvalidClosingChecklistExporter(ITempFileCacheManager tempFileCacheManager, IHostingEnvironment env, IHttpContextAccessor httpContextAccessor)
            : base(tempFileCacheManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }
        public string ExportToFile(List<ClosingChecklistExcellImportDto> inputs)
        {
            if (!Directory.Exists(_env.WebRootPath + "/Uploads-Attachments"))
            {
                Directory.CreateDirectory(_env.WebRootPath + "/Uploads-Attachments");
            }
            string filePath = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ".xlsx";
            var path = Path.Combine(Path.Combine(_env.WebRootPath, "Uploads-Attachments"), filePath);
            var result = CreateExcelPackage(
                "InvalidClosingChecklistImportListExport.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("InvalidClosingChecklistImports"));
                    sheet.OutLineApplyStyle = true;


                    AddHeader(
                          sheet,
                                     L("CategoryName"),
                                     L("TaskName"),
                                     L("AssigneeEmail"),
                                     L("Frequency"),
                                     L("NoOfMonths"),
                                     L("DueOn"),
                                     L("EndOfMonth"),
                                     L("DayBeforeAfter"),
                                     L("Status"),
                                     L("Instruction"),
                                     L("Error")

                                    );

                    AddObjects(
                         sheet, 2, inputs,
                          _ => _.CategoryName,
                         _ => _.TaskName,
                          _ => _.AssigneeEmail,
                          _ => _.Frequency,
                          _ => _.NoOfMonths,
                          _ => _.DueOn,
                          _ => _.EndOfMonth,
                          _ => _.DayBeforeAfter,
                          _ => _.Status,
                          _ => _.Instruction,
                         _ => _.Exception
                        );

                    for (var i = 1; i <= 10; i++)
                    {
                        sheet.Column(i).AutoFit();
                    }
                    excelPackage.SaveAs(new FileInfo(path));
                });
            return "Uploads-Attachments/" + filePath; ;

        }
    }
}
