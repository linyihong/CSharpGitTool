using GitTools;
using Microsoft.AspNetCore.Mvc;

namespace TestWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var gitHelper = new GitHelper();
            var projectNames = gitHelper.ProjectNames;
            ViewData["ProjectNames"] = projectNames;

            return View();
        }
    }
}
