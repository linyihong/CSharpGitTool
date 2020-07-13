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
            var authorCommits = gitHelper.GetCommits(projectNames[0], "develop");
            ViewData["ProjectNames"] = projectNames;
            ViewData["Commits"] = authorCommits;


            return View();
        }
    }
}
