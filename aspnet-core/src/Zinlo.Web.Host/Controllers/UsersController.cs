using Abp.AspNetCore.Mvc.Authorization;
using Zinlo.Authorization;
using Zinlo.Storage;
using Abp.BackgroundJobs;

namespace Zinlo.Web.Controllers
{
    [AbpMvcAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UsersController : UsersControllerBase
    {
        public UsersController(IBinaryObjectManager binaryObjectManager, IBackgroundJobManager backgroundJobManager)
            : base(binaryObjectManager, backgroundJobManager)
        {
        }
    }
}