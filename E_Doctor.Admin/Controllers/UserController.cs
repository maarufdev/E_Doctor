using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Admin.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
