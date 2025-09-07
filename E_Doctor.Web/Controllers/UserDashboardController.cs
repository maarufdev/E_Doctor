using E_Doctor.Infrastructure.WebHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Web.Controllers
{
    [Authorize]
    public class UserDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}