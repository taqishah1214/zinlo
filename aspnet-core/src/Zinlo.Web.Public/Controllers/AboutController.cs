using Microsoft.AspNetCore.Mvc;
using Zinlo.Web.Controllers;

namespace Zinlo.Web.Public.Controllers
{
    public class AboutController : ZinloControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}