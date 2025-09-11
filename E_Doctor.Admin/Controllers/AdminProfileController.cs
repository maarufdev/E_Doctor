using E_Doctor.Application.Interfaces.Features.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Web.Controllers
{
    [Authorize]
    public class AdminProfileController : Controller
    {
        private readonly IUserManagerService _userManagerService;
        public AdminProfileController(IUserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _userManagerService.GetUserProfile();
            return View(result);
        }
    }
}
