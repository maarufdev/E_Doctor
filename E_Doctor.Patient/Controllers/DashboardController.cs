using E_Doctor.Application.Interfaces.Features.Patient.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Patient.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IPatientDashboardService _patientDashboardService;
        public DashboardController(IPatientDashboardService patientDashboardService)
        {
            _patientDashboardService = patientDashboardService;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _patientDashboardService.GetDashboardDetails();

            return View(result.Value);
        }
    }
}
