using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Patient.Controllers
{
    public class DiagnosisController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
