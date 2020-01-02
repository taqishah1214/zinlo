using System.Collections.Generic;
using Zinlo.Authorization.Users.Importing.Dto;
using Zinlo.Dto;

namespace Zinlo.Authorization.Users.Importing
{
    public interface IInvalidUserExporter
    {
        FileDto ExportToFile(List<ImportUserDto> userListDtos);
    }
}
