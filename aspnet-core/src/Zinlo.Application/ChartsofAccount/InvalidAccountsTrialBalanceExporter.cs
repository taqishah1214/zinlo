using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.ChartsofAccount.Importing;
using Zinlo.DataExporting.Excel.EpPlus;
using Zinlo.Dto;
using Zinlo.Storage;

namespace Zinlo.ChartsofAccount
{

    public class InvalidAccountsTrialBalanceExporter : EpPlusExcelExporterBase, IInvalidAccountsTrialBalanceExporter
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _env;
        public InvalidAccountsTrialBalanceExporter(ITempFileCacheManager tempFileCacheManager, IHostingEnvironment env, IHttpContextAccessor httpContextAccessor)
            : base(tempFileCacheManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _env = env;
        }
        public string ExportToFile(List<ChartsOfAccountsTrialBalanceExcellImportDto> inputs)
        {
            if (!Directory.Exists(_env.WebRootPath + "/Uploads-Attachments"))
            {
                Directory.CreateDirectory(_env.WebRootPath + "/Uploads-Attachments");
            }
            string filePath = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + Guid.NewGuid().ToString("N") + ".xlsx";
            var path = Path.Combine(Path.Combine(_env.WebRootPath, "Uploads-Attachments"), filePath);
            var result = CreateExcelPackage(
                "InvalidAccontTrialBalanceImportListExport.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("InvalidTrialBalanceImports"));
                    sheet.OutLineApplyStyle = true;


                    AddHeader(
                          sheet,
                                     L("AccountNumber"),
                                     L("AccountName"),
                                     L("Balance"),
                                     L("Error")

                                    );

                    AddObjects(
                         sheet, 2, inputs,
                          _ => _.AccountNumber,
                         _ => _.AccountName,
                          _ => _.Balance,
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













    //public class InvalidAccountsTrialBalanceExporter : EpPlusExcelExporterBase, IInvalidAccountsTrialBalanceExporter
    // {
    //     public InvalidAccountsTrialBalanceExporter(ITempFileCacheManager tempFileCacheManager)
    //       : base(tempFileCacheManager)
    //     {
    //     }
    //     public FileDto ExportToFile(List<ChartsOfAccountsTrialBalanceExcellImportDto> inputs)
    //     {

    //         return CreateExcelPackage(
    //             "InvalidAccontTrialBalanceImportListExport.xlsx",
    //             excelPackage =>
    //             {
    //                 var sheet = excelPackage.Workbook.Worksheets.Add(L("InvalidTrialBalanceImports"));
    //                 sheet.OutLineApplyStyle = true;


    //                 AddHeader(
    //                                 sheet,
    //                                 L("AccountNumber"),
    //                                 L("AccountName"),                                  
    //                                 L("Balance"),                              
    //                                 L("Error")

    //                                 );

    //                 AddObjects(
    //                     sheet, 2, inputs,
    //                      _ => _.AccountNumber,
    //                     _ => _.AccountName,                      
    //                      _ => _.Balance,
    //                     _ => _.Exception
    //                     );

    //                 for (var i = 1; i <= 9; i++)
    //                 {
    //                     sheet.Column(i).AutoFit();
    //                 }
    //             });
    //     }


    // }
}
