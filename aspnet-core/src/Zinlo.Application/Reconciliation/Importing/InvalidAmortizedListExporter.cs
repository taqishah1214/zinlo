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
    public class InvalidAmortizedListExporter : EpPlusExcelExporterBase, IInvalidAmortizedExporter
    {
        private readonly IHostingEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InvalidAmortizedListExporter(ITempFileCacheManager tempFileCacheManager,
            IHostingEnvironment env, IHttpContextAccessor httpContextAccessor)
            : base(tempFileCacheManager)
        {
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public string ExportToFile(List<AmortizedExcelImportDto> inputs)
        {
            if (!Directory.Exists(_env.WebRootPath + "/Uploads-Attachments"))
            {
                Directory.CreateDirectory(_env.WebRootPath + "/Uploads-Attachments");
            }
            string filePath = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ".xlsx";
            var path = Path.Combine(Path.Combine(_env.WebRootPath, "Uploads-Attachments"), filePath);
            var result = CreateExcelPackage(
                "InvalidAmortizedImportListExport.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("InvalidAmortizedListImports"));
                    sheet.OutLineApplyStyle = true;


                    AddHeader(
                          sheet,
                                     L("InvoiceNo"),
                                     L("JournalEntryNo"),
                                     L("StartDate"),
                                     L("EndDate"),
                                     L("Amount"),
                                     L("Description"),
                                     L("Criteria"),
                                     L("Error")
                                    );

                    AddObjects(
                         sheet, 2, inputs,
                          _ => _.InvoiceNo,
                         _ => _.JournalEntryNo,
                          _ => _.StartDate,
                          _ => _.EndDate,
                          _ => _.Amount,
                          _ => _.Description,
                          _=> _.Criteria,
                          _=>_.Exception
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
