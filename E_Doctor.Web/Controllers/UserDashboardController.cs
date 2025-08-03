using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Web.Controllers
{
    public class UserDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
