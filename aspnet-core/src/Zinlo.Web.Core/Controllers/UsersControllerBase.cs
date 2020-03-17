using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Abp.IO.Extensions;
using Abp.UI;
using Abp.Web.Models;
using Zinlo.Authorization.Users.Dto;
using Zinlo.Storage;
using Abp.BackgroundJobs;
using Zinlo.Authorization;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Runtime.Session;
using Zinlo.Authorization.Users.Importing;
using Zinlo.ChartsofAccount;
using Zinlo.ChartsofAccount.Importing;
using System.Net;
using Zinlo.ChartsofAccount.Dtos;

namespace Zinlo.Web.Controllers
{
    public abstract class UsersControllerBase : ZinloControllerBase
    {
        protected readonly IBinaryObjectManager BinaryObjectManager;
        protected readonly IBackgroundJobManager BackgroundJobManager;

        protected UsersControllerBase(
            IBinaryObjectManager binaryObjectManager,
            IBackgroundJobManager backgroundJobManager)
        {
            BinaryObjectManager = binaryObjectManager;
            BackgroundJobManager = backgroundJobManager;
        }

        [HttpPost]
        [AbpMvcAuthorize(AppPermissions.Pages_Administration_Users_Create)]
        public async Task<JsonResult> ImportFromExcel()
        {
            try
            {
                var file = Request.Form.Files.First();

                if (file == null)
                {
                    throw new UserFriendlyException(L("File_Empty_Error"));
                }

                if (file.Length > 1048576 * 100) //100 MB
                {
                    throw new UserFriendlyException(L("File_SizeLimit_Error"));
                }

                byte[] fileBytes;
                using (var stream = file.OpenReadStream())
                {
                    fileBytes = stream.GetAllBytes();
                }

                var tenantId = AbpSession.TenantId;
                var fileObject = new BinaryObject(tenantId, fileBytes);

                await BinaryObjectManager.SaveAsync(fileObject);

                await BackgroundJobManager.EnqueueAsync<ImportUsersToExcelJob, ImportUsersFromExcelJobArgs>(new ImportUsersFromExcelJobArgs
                {
                    TenantId = tenantId,
                    BinaryObjectId = fileObject.Id,
                    User = AbpSession.ToUserIdentifier()
                });

                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        public async Task<JsonResult> ImportAccountsFromExcel(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                byte[] fileBytes;
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    fileBytes = stream.GetAllBytes();
                }

                var tenantId = AbpSession.TenantId;
                var fileObject = new BinaryObject(tenantId, fileBytes);

                await BinaryObjectManager.SaveAsync(fileObject);

                await BackgroundJobManager.EnqueueAsync<ImportChartsOfAccountToExcelJob, ImportChartsOfAccountFromExcelJobArgs>(new ImportChartsOfAccountFromExcelJobArgs
                {
                    TenantId = tenantId,
                    BinaryObjectId = fileObject.Id,                 
                    User = AbpSession.ToUserIdentifier()
                });

                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        [HttpGet]
        public async Task<JsonResult> ImportAccountsTrialBalanceFromExcel(string url)
        {
            try
            {              
                WebRequest request = WebRequest.Create(url);
                byte[] fileBytes;
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    fileBytes = stream.GetAllBytes();
                }

                var tenantId = AbpSession.TenantId;
                var fileObject = new BinaryObject(tenantId, fileBytes);

                await BinaryObjectManager.SaveAsync(fileObject);

                await BackgroundJobManager.EnqueueAsync<ImportChartsOfAccountTrialBalanceToExcelJob, ImportChartsOfAccountTrialBalanceFromExcelJobArgs>(new ImportChartsOfAccountTrialBalanceFromExcelJobArgs
                {
                    TenantId = tenantId,
                    BinaryObjectId = fileObject.Id,
                    User = AbpSession.ToUserIdentifier()
                });

                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
    }
}
