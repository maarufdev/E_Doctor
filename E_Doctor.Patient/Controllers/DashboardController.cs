using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Patient.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        public DashboardController()
        {
        }
        public IActionResult Index()
        {
            //var result = await _patientDashboardService.GetDashboardDetails();

            //return View(result.Value);

            return View();
        }
    }
}
