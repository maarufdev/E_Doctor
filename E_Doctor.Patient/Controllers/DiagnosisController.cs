using E_Doctor.Application.DTOs.Diagnosis;
using E_Doctor.Application.Interfaces.Features.Admin.Diagnosis;
using E_Doctor.Application.Interfaces.Features.Patient.Diagnosis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace E_Doctor.Patient.Controllers
{
    [Authorize]
    public class DiagnosisController : Controller
    {
        private readonly IPatientService _patientService;
        public DiagnosisController(IPatientService patientService)
        {
            _patientService = patientService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportRulesConfiguration([FromForm] IFormFile file)
        {
            if (file is null) return BadRequest();

            string jsonContent;
            
            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            jsonContent = await reader.ReadToEndAsync();

            if(string.IsNullOrWhiteSpace(jsonContent)) return BadRequest();

            var result = await _patientService.MigrateRules(jsonContent);

            return Ok(result);
        }

        public async Task<IActionResult> GetSymptoms()
        {
            return Ok(await _patientService.GetSymtoms());
        }

        [HttpPost]
        public async Task<IActionResult> RunDiagnosis([FromBody] List<RunDiagnosisDTO> requestDTO)
        {
            if (requestDTO.Count == 0) return BadRequest("Please provide diagnosis");

            var result = await _patientService.RunDiagnosis(requestDTO);

            return Ok(result);
        }

        public async Task<IActionResult> GetDiagnosis()
        {
            return Ok(await _patientService.GetDiagnosis());
        }

        public async Task<IActionResult> GetDiagnosisById(int diagnosisId)
        {
            return Ok(await _patientService.GetDiagnosisById(diagnosisId));
        }
    }
}
