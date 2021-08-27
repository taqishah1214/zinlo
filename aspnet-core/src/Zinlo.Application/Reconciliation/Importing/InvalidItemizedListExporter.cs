using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.DataExporting.Excel.EpPlus;
using Zinlo.Reconciliation.Dtos;
using Zinlo.Reconciliation.Importing;
using Zinlo.Storage;
using System.IO;

namespace Zinlo.Reconciliation.Importing
{
    public class InvalidItemizedListExporter : EpPlusExcelExporterBase, IInvalidItemizedExporter
    {
        private readonly IHostingEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InvalidItemizedListExporter(ITempFileCacheManager tempFileCacheManager,
            IHostingEnvironment env, IHttpContextAccessor httpContextAccessor)
            : base(tempFileCacheManager)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public string ExportToFile(List<ItemizedExcelImportDto> inputs)
        {
            if (!Directory.Exists(_env.WebRootPath + "/Uploads-Attachments"))
            {
                Directory.CreateDirectory(_env.WebRootPath + "/Uploads-Attachments");
            }
            string filePath = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ".xlsx";
            var path = Path.Combine(Path.Combine(_env.WebRootPath, "Uploads-Attachments"), filePath);
            var result = CreateExcelPackage(
                "InvalidItemizedImportListExport.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("InvalidItemizedListImports"));
                    sheet.OutLineApplyStyle = true;


                    AddHeader(
                          sheet,
                                     L("InvoiceNo"),
                                     L("JournalEntryNo"),
                                     L("Date"),
                                     L("Amount"),
                                     L("Description"),
                                     L("Error")

                                    );

                    AddObjects(
                         sheet, 2, inputs,
                          _ => _.InoviceNo,
                         _ => _.JournalEntryNo,
                          _ => _.Date,
                          _ => _.Amount,
                          _ => _.Description,
                          _ => _.Exception                         
                        );

                    for (var i = 1; i <= 10; i++)
                    {
                        sheet.Column(i).AutoFit();
                    }
                    excelPackage.SaveAs(new FileInfo(path));
                });
            return "Uploads-Attachments/" + filePath;

        }
    }
}
