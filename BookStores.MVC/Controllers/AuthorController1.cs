using Microsoft.AspNetCore.Mvc;

namespace BookStores.MVC.Controllers
{
    public class AuthorController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
