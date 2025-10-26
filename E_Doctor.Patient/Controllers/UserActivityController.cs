using E_Doctor.Application.DTOs.UserActivity;
using E_Doctor.Application.Interfaces.Features.UserActivity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Patient.Controllers
{
    [Authorize]
    public class UserActivityController(IUserActivityService userActivity) : Controller
    {
        private readonly IUserActivityService _userActivity = userActivity;
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetUserActivity([FromQuery] GetPatientUserActivityRequest request)
        {
            var result = await _userActivity.GetUserActivitiesById(request);

            if (result.IsFailure) return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}
