using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Patient.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
