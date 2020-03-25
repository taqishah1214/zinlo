using Abp.Runtime.Session;
using Abp.Timing.Timezone;
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
    public class ChartsOfAccountsTrialBalanceExcelExporter : EpPlusExcelExporterBase, IChartsOfAccountsTrialBalanceExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ChartsOfAccountsTrialBalanceExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager)
            : base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        //public FileDto ExportToExcell(List<ChartsOfAccountsTrialBalanceExcellImportDto> accountsTrialBalanceDtos)
        //{
        //    throw new NotImplementedException();
        //}

        public FileDto ExportToExcell(List<ChartsOfAccountsTrialBalanceExcellImportDto> accountsListDtos)
        {
            return CreateExcelPackage(
                "TrialBalance.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("ChartsOfAccounts"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("AccountName"),
                        L("AccountNumber"),
                          L("Balance")
                        );

                    AddObjects(
                        sheet, 2, accountsListDtos,
                        _ => _.AccountName,
                        _ => _.AccountNumber,
                          _ => _.Balance
                        );

                    for (var i = 1; i <= 9; i++)
                    {
                        sheet.Column(i).AutoFit();
                    }
                });
        }


    }
}
