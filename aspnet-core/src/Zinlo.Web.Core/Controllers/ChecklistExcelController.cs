using Abp.BackgroundJobs;
using Abp.IO.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Zinlo.ClosingChecklist.Importing;
using Zinlo.Storage;

namespace Zinlo.Web.Controllers
{
    public class ChecklistExcelController : ZinloControllerBase
    {
        protected readonly IBinaryObjectManager BinaryObjectManager;
        protected readonly IBackgroundJobManager BackgroundJobManager;

        public ChecklistExcelController(
            IBinaryObjectManager binaryObjectManager,
            IBackgroundJobManager backgroundJobManager)
        {
            BinaryObjectManager = binaryObjectManager;
            BackgroundJobManager = backgroundJobManager;
        }

        [HttpGet]
        public async Task<JsonResult> ImportChecklistFromExcel(string url, string monthSelected)
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

                BackgroundJobManager.Enqueue<ImportClosingChecklistToExcelJob, ImportClosingChecklistFromExcelJobArgs>(new ImportClosingChecklistFromExcelJobArgs
                {
                    TenantId = tenantId,
                    BinaryObjectId = fileObject.Id,
                    User = AbpSession.ToUserIdentifier(),
                    selectedMonth = selectedMonth,
                    url = url
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