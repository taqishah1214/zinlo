using System;
using System.Collections.Generic;
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
        public InvalidAccountsTrialBalanceExporter(ITempFileCacheManager tempFileCacheManager)
          : base(tempFileCacheManager)
        {
        }
        public FileDto ExportToFile(List<ChartsOfAccountsTrialBalanceExcellImportDto> inputs)
        {

            return CreateExcelPackage(
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
                });
        }

     
    }
}
