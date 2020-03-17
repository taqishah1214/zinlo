using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.DataExporting.Excel.EpPlus;
using Zinlo.Dto;
using Zinlo.Storage;

namespace Zinlo.ChartsofAccount
{

    public class InvalidAccountsExporter : EpPlusExcelExporterBase, IInvalidAccountsExcellExporter
    {
        public InvalidAccountsExporter(ITempFileCacheManager tempFileCacheManager)
            : base(tempFileCacheManager)
        {
        }

        public FileDto ExportToFile(List<ChartsOfAccountsExcellImportDto> accountsListDtos)
        {
            return CreateExcelPackage(
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
                        L("Exception")
                      
                        );

                    AddObjects(
                        sheet, 2, accountsListDtos,
                        _ => _.AccountName,
                        _ => _.AccountNumber,
                        _ => _.AccountType,
                        _ => _.AssignedUser,
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
