using E_Doctor.Application.Interfaces.Features.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Patient.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IUserManagerService _userManagerService;
        public DashboardController(IUserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }
        public async Task<IActionResult> Index()
        {
            await _userManagerService.UpdateUserLoginDate();

            return View();
        }
    }
}
