using Abp.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace Zinlo.Web.Controllers
{
    public class HomeController : ZinloControllerBase
    {
        [DisableAuditing]
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Ui");
        }
    }
}
