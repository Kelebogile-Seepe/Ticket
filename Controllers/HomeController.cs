using Microsoft.AspNetCore.Mvc;

namespace BeyonceConcert.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
