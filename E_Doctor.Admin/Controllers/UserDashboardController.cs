using E_Doctor.Application.Interfaces.Features.Admin.Dashboard;
using E_Doctor.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Web.Controllers
{
    [Authorize(Roles = RoleConstants.Admin)]
    public class UserDashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        public UserDashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;   
        }
        public async Task<IActionResult> Index()
        {
            var result = await _dashboardService.GetDashboardDetails();

            return View(result.Value);
        }
    }
}