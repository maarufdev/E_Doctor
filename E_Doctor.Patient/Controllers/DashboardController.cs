using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Patient.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
