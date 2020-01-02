using Microsoft.AspNetCore.Mvc;
using Zinlo.Web.Controllers;

namespace Zinlo.Web.Public.Controllers
{
    public class HomeController : ZinloControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}