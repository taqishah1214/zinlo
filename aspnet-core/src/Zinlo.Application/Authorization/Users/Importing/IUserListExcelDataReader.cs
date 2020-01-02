using System.Collections.Generic;
using Zinlo.Authorization.Users.Importing.Dto;
using Abp.Dependency;

namespace Zinlo.Authorization.Users.Importing
{
    public interface IUserListExcelDataReader: ITransientDependency
    {
        List<ImportUserDto> GetUsersFromExcel(byte[] fileBytes);
    }
}
