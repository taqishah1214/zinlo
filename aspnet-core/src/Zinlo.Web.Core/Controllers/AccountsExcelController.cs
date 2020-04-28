using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Abp.IO.Extensions;
using Abp.UI;
using Abp.Web.Models;
using Zinlo.Storage;
using Abp.BackgroundJobs;
using Abp.Runtime.Session;
using Zinlo.ChartsofAccount;
using Zinlo.ChartsofAccount.Importing;
using System.Net;
using System;
using System.Globalization;

namespace Zinlo.Web.Controllers
{
    public class AccountsExcelController : ZinloControllerBase
    {
        protected readonly IBinaryObjectManager BinaryObjectManager;
        protected readonly IBackgroundJobManager BackgroundJobManager;

        public AccountsExcelController(
            IBinaryObjectManager binaryObjectManager,
            IBackgroundJobManager backgroundJobManager)
        {
            BinaryObjectManager = binaryObjectManager;
            BackgroundJobManager = backgroundJobManager;
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

               await  BackgroundJobManager.EnqueueAsync<ImportChartsOfAccountToExcelJob, ImportChartsOfAccountFromExcelJobArgs>(new ImportChartsOfAccountFromExcelJobArgs
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
        public async Task<JsonResult> ImportAccountsTrialBalanceFromExcel(string url, string monthSelected)
        {
            try
            {           
                string date = monthSelected.Substring(4, 11);
                string s = DateTime.ParseExact(date, "MMM dd yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                DateTime selectedMonth = Convert.ToDateTime(s);
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

                 BackgroundJobManager.Enqueue<ImportChartsOfAccountTrialBalanceToExcelJob, ImportChartsOfAccountTrialBalanceFromExcelJobArgs>(new ImportChartsOfAccountTrialBalanceFromExcelJobArgs
                {
                    TenantId = tenantId,
                    BinaryObjectId = fileObject.Id,
                    User = AbpSession.ToUserIdentifier(),
                    selectedMonth = selectedMonth
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
