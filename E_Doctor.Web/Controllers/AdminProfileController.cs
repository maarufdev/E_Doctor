using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Web.Controllers
{
    public class AdminProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
