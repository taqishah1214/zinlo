﻿using System.Collections.Generic;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Zinlo.Authorization.Users.Dto;
using Zinlo.DataExporting.Excel.EpPlus;
using Zinlo.Dto;
using Zinlo.Storage;

namespace Zinlo.Authorization.Users.Exporting
{
    public class UserListExcelExporter : EpPlusExcelExporterBase, IUserListExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public UserListExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager)
            : base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<UserListDto> userListDtos)
        {
            return CreateExcelPackage(
                "UserList.xlsx",
                excelPackage =>
                {
                    var sheet = excelPackage.Workbook.Worksheets.Add(L("Users"));
                    sheet.OutLineApplyStyle = true;

                    AddHeader(
                        sheet,
                        L("Name"),
                        L("Surname"),
                        L("UserName"),
                        L("PhoneNumber"),
                        L("EmailAddress"),
                        L("EmailConfirm"),
                        L("Roles"),
                        L("Active"),
                        L("CreationTime")
                        );

                    AddObjects(
                        sheet, 2, userListDtos,
                        _ => _.Name,
                        _ => _.Surname,
                        _ => _.UserName,
                        _ => _.PhoneNumber,
                        _ => _.EmailAddress,
                        _ => _.IsEmailConfirmed,
                        _ => _.Roles.Select(r => r.RoleName).JoinAsString(", "),
                        _ => _.IsActive,
                        _ => _timeZoneConverter.Convert(_.CreationTime, _abpSession.TenantId, _abpSession.GetUserId())
                        );

                    //Formatting cells

                    var creationTimeColumn = sheet.Column(9);
                    creationTimeColumn.Style.Numberformat.Format = "yyyy-mm-dd";

                    for (var i = 1; i <= 9; i++)
                    {
                        sheet.Column(i).AutoFit();
                    }
                });
        }
    }
}
