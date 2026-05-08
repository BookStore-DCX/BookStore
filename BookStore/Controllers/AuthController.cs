using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
