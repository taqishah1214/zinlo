﻿using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using System;
using System.Collections.Generic;
using System.Text;
using Zinlo.ChartsofAccount.Dtos;
using Zinlo.DataExporting.Excel.EpPlus;
using Zinlo.Dto;
using Zinlo.Storage;

namespace Zinlo.ChartsofAccount
{
    public class ChartsOfAccountsListExcelExporter : EpPlusExcelExporterBase, IChartsOfAccountsListExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public ChartsOfAccountsListExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager)
            : base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<ChartsOfAccountsExcellExporterDto> accountsListDtos)
        {
            return CreateExcelPackage(
                "ChartOfAccountsList.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("ChartsOfAccounts"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("AccountName"),
                        L("AccountNumber"),
                        L("AccountType"),
                        L("AssignedUser")


                        );

                    AddObjects(
                        sheet, 2, accountsListDtos,
                        _ => _.AccountName,
                        _ => _.AccountNumber,
                         _ => _.AccountType,
                          _ => _.AssignedUser



                        );

                    //Formatting cells

                    //var creationTimeColumn = sheet.Column(9);
                    //creationTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";

                    for (var i = 1; i <= 9; i++)
                    {
                        sheet.Column(i).AutoFit();
                    }
                });
        }
    }
}
