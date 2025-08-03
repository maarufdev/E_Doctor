using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Web.Controllers
{
    public class AdminDiagnosisController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
