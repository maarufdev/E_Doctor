using E_Doctor.Application.DTOs.Common.UserAccountDTOs;
using E_Doctor.Application.Interfaces.Features.Common;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Patient.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserManagerService _userService;
        public AccountController(IUserManagerService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var result = await _userService.Login(login);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.Value);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO register)
        {
            var result = await _userService.Register(register);

            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return Ok(result.IsSuccess);
        }

        public async Task<IActionResult> Logout()
        {
            await _userService.Logout();

            return RedirectToAction("Login", "Account");
        }

        public IActionResult ForgotPassword() => View();

        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            var result = await _userService.ResetPassword(resetPasswordDTO);

            if (result.IsFailure) return BadRequest(result.Error);

            return Ok(result.IsSuccess);
        }
    }
}
