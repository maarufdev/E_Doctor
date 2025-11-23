using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Patient.Helpers;
using E_Doctor.Patient.ViewModels.PatientVM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace E_Doctor.Patient.Controllers
{

    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserManagerService _userManagerService;
        public ProfileController(IUserManagerService userManagerService)
        {
            _userManagerService = userManagerService;
        }
        public async Task<IActionResult> Index()
        {
            var result = await _userManagerService.GetUserProfile();
            await _userManagerService.UpdateUserLoginDate();

            return View(result);
        }

        public async Task<IActionResult> UpsertProfile()
        {
            var userId = await _userManagerService.GetUserId();
            
            if(userId is null)
            {
                return View();
            }

            var result = await _userManagerService.GetUserDetailsByUserId((int)userId);

            if (result.IsFailure)
            {
                return View();
            }

            var vm = result.Value.ToUpsertViewModal();
            
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> UpsertProfile(UpsertProfileVM vm)
        {
            if (!string.IsNullOrWhiteSpace(vm.GetMissingFields()))
            {
                ViewData["MissingFields"] = vm.GetMissingFields();

                return View(vm);
            }

            var dto = vm.ToDto();

            var result = await _userManagerService.InsertOrUpdateUserDetails(dto);

            if (result.IsFailure)
            {
                ViewData["Error"] = result.Error;
                return View(vm);
            }

            return RedirectToAction("Index");
        }

    }
}
