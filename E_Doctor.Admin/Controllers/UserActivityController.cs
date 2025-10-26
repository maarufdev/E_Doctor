using E_Doctor.Application.Constants;
using E_Doctor.Application.DTOs.UserActivity;
using E_Doctor.Application.Interfaces.Features.UserActivity;
using E_Doctor.Infrastructure.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Admin.Controllers
{
    [Authorize(Roles = RoleConstants.Admin)]
    public class UserActivityController(
        IUserActivityService userActivityService
        ) : Controller
    {
        private readonly IUserActivityService _userActivityService = userActivityService;
        public IActionResult Index(UserActivityTab? tabName = UserActivityTab.Users)
        {
            var pageTitle = "Users Activity";
            ViewBag.ActiveTab = tabName;            
            switch (tabName)
            {
                case UserActivityTab.Admin:
                    pageTitle = "Admin Activity";
                    break;
                default:
                    break;
            }

            ViewBag.PageTitle = pageTitle;
            return View();
        }

        public async Task<IActionResult> GetPatientUserActivities([FromQuery] GetPatientUserActivityRequest request)
        {
            var result = await _userActivityService.GetAllUserActivities(request);

            if (result.IsFailure) return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}
