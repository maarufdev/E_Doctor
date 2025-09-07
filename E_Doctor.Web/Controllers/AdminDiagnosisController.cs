using E_Doctor.Application.DTOs.Diagnosis;
using E_Doctor.Application.Interfaces.Features.Admin.Diagnosis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Web.Controllers
{
    [Authorize]
    public class AdminDiagnosisController : Controller
    {
        private readonly IDiagnosisService _diagnosisService;
        public AdminDiagnosisController(IDiagnosisService diagnosisService)
        {
            _diagnosisService = diagnosisService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetDiagnosis()
        {
            return Ok(await  _diagnosisService.GetDiagnosis());
        }

        public async Task<IActionResult> RunDiagnosis([FromBody] List<RunDiagnosisDTO> requestDTO)
        {
            if (requestDTO.Count == 0) return BadRequest("Please provide diagnosis");

            var result = await _diagnosisService.RunDiagnosis(requestDTO);

            return Ok(result);
        }
    }
}