using E_Doctor.Application.Interfaces.Features.Common;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Admin.Controllers
{
    public class CommonController(IUserManagerService userService) : Controller
    {
        private readonly IUserManagerService _userManagerService = userService;
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> PatientDetailsView(int userInfoId)
        {
            var userDetails = await _userManagerService.GetUserDetailsByUserInfoId(userInfoId);

            return PartialView("_PatientDetailsView", userDetails.Value);
        }
    }
}
