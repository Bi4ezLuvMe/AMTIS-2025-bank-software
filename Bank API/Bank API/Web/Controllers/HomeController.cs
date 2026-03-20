
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bank_API.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return Content("Bank API Simulator is running. Start by calling POST /sessions");
        }

        public IActionResult Privacy()
        {
            return View();
        }

      
    }
}
