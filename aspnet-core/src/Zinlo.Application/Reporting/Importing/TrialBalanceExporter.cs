using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.DataExporting.Excel.EpPlus;
using Zinlo.Dto;
using Zinlo.Reporting.Dtos;
using Zinlo.Storage;

namespace Zinlo.Reporting.Importing
{
    public class TrialBalanceExporter : EpPlusExcelExporterBase , ITrialBalanceExporter
    {

        public TrialBalanceExporter(ITempFileCacheManager tempFileCacheManager)
           : base(tempFileCacheManager)
        {
        }
        public FileDto ExportToFile(List<CompareTrialBalanceViewDto> List)
        {
            return CreateExcelPackage(
                "Balance Compare List.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Tasks"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("AccountName"),
                        L("AccountNumber"),
                        L("FirstMonthBalance"),
                        L("SecondMonthBalance")
                       
                    );

                    AddObjects(
                        sheet, 2, List,
                        _ => _.AccountName,
                        _ => _.AccountNumber,
                        _ => _.FirstMonthBalance,
                        _ => _.SecondMonthBalance
                    );

                    //Formatting cells

                    var creationTimeColumn = sheet.Column(4);
                    creationTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";

                    for (var i = 1; i <= 4; i++)
                    {
                        sheet.Column(i).AutoFit();
                    }
                });
        }
    }
}
