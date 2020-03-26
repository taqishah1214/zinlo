using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.DataExporting.Excel.EpPlus;
using Zinlo.Dto;
using Zinlo.Storage;

namespace Zinlo.ChartsofAccount
{


    public class InvalidAccountsExporter : EpPlusExcelExporterBase, IInvalidAccountsExcellExporter
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _env;
        public InvalidAccountsExporter(ITempFileCacheManager tempFileCacheManager, IHostingEnvironment env, IHttpContextAccessor httpContextAccessor)
            : base(tempFileCacheManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }

      

        public string ExportToFile(List<ChartsOfAccountsExcellImportDto> accountsListDtos)
        {
            if (!Directory.Exists(_env.WebRootPath + "/Uploads-Attachments"))
            {
                Directory.CreateDirectory(_env.WebRootPath + "/Uploads-Attachments");
            }
            string filePath = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N")+ ".xlsx";
            var path = Path.Combine(Path.Combine(_env.WebRootPath, "Uploads-Attachments"), filePath);
            var result = CreateExcelPackage(
                "InvalidAccountsImportList.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("InvalidAccountsImports"));
                    sheet.OutLineApplyStyle = true;


                    AddHeader(
                                   sheet,
                        L("AccountName"),
                        L("AccountNumber"),
                        L("AccountType"),
                        L("AccountSubType"),
                        L("AssignedUser"),
                        L("ReconciliationType"),
                         L("ReconciliationAs"),
                        L("Error")

                                    );

                    AddObjects(
                          sheet, 2, accountsListDtos,
                        _ => _.AccountName,
                        _ => _.AccountNumber,
                        _ => _.AccountType,
                        _ => _.AccountSubType,
                         _ => _.AssignedUser,
                          _ => _.ReconciliationType,
                           _ => _.ReconciliationAs,
                        _ => _.Exception
                        );

                    for (var i = 1; i <= 9; i++)
                    {
                        sheet.Column(i).AutoFit();
                    }
                    excelPackage.SaveAs(new FileInfo(path));
                });
            return path;

        }      
    }

}
