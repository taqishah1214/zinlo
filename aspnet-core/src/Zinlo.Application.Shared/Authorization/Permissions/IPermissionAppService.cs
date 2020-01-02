using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Zinlo.Authorization.Permissions.Dto;

namespace Zinlo.Authorization.Permissions
{
    public interface IPermissionAppService : IApplicationService
    {
        ListResultDto<FlatPermissionWithLevelDto> GetAllPermissions();
    }
}
