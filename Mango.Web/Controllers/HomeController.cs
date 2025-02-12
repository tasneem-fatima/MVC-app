using Microsoft.AspNetCore.Mvc;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
     

        public async Task<IActionResult> Index()
        {
           

            return View();
        }

        
    }
}