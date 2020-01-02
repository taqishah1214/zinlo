using System.Collections.Generic;
using Zinlo.Authorization.Users.Dto;
using Zinlo.Dto;

namespace Zinlo.Authorization.Users.Exporting
{
    public interface IUserListExcelExporter
    {
        FileDto ExportToFile(List<UserListDto> userListDtos);
    }
}