using Abp.BackgroundJobs;
using Abp.IO.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Globalization;
using Zinlo.Reconciliation.Importing;
using Zinlo.Storage;

namespace Zinlo.Web.Controllers
{
    public class ReconciliationExcelController : ZinloControllerBase
    {
        protected readonly IBinaryObjectManager BinaryObjectManager;
        protected readonly IBackgroundJobManager BackgroundJobManager;

        public ReconciliationExcelController(
            IBinaryObjectManager binaryObjectManager,
            IBackgroundJobManager backgroundJobManager)
        {
            BinaryObjectManager = binaryObjectManager;
            BackgroundJobManager = backgroundJobManager;
        }

        [HttpGet]
        public async Task<JsonResult> ImportItemizedItems(string url, string monthSelected, string chartsOfAccountId)
        {
            try
            {
                string date = monthSelected.Substring(4, 11);
                string s = DateTime.ParseExact(date, "MMM dd yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                DateTime selectedMonth = Convert.ToDateTime(s);
                long accountId = long.Parse(chartsOfAccountId);

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

                await BackgroundJobManager.EnqueueAsync<ImportItemizedToExcelJob, ImportItemizedFromExcelJobArgs>(new ImportItemizedFromExcelJobArgs
                {
                    TenantId = tenantId,
                    BinaryObjectId = fileObject.Id,
                    User = AbpSession.ToUserIdentifier(),
                    Url = url,
                    ChartsofAccountsId = accountId,
                    SelectedMonth = selectedMonth
                });

                return Json(new AjaxResponse(new { }));
            }
            catch (UserFriendlyException ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpGet]
        public async Task<JsonResult> ImportAmortizedItems(string url, string monthSelected, string chartsOfAccountId)
        {
            try
            {
                string date = monthSelected.Substring(4, 11);
                string s = DateTime.ParseExact(date, "MMM dd yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                DateTime selectedMonth = Convert.ToDateTime(s);
                long accountId = long.Parse(chartsOfAccountId);

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

                await BackgroundJobManager.EnqueueAsync<ImportAmortizedToExcelJob, ImportAmortizedFromExcelJobArgs>(new ImportAmortizedFromExcelJobArgs
                {
                    TenantId = tenantId,
                    BinaryObjectId = fileObject.Id,
                    User = AbpSession.ToUserIdentifier(),
                    Url = url,
                    ChartsofAccountsId = accountId,
                    SelectedMonth = selectedMonth
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
