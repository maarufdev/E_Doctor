using E_Doctor.Application.DTOs.Diagnosis;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Web.Controllers
{
    public class AdminDiagnosisController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RunDiagnosis([FromBody] IEnumerable<RunDiagnosisDTO> requestDTO)
        {
            return Ok(requestDTO);
        }
    }
}
