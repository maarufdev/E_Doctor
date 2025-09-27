using E_Doctor.Application.Constants;
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
        public IActionResult Index(AdminDiagnosisTab? tabName = AdminDiagnosisTab.Diagnosis)
        {
            ViewBag.ActiveTab = tabName;
            var pageTitle = "Diagnosis";

            switch (tabName)
            {
                case AdminDiagnosisTab.Consultation:
                    pageTitle = "Consulation";
                    break;
                default:
                    pageTitle = "Diagnosis";
                    break;
            }

            ViewBag.PageTitle = pageTitle;

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
        public async Task<IActionResult> RunDiagnosis([FromBody] RunDiagnosisDTO requestDTO)
        {
            if (requestDTO is null) return BadRequest("Please provide diagnosis");

            var result = await _patientService.RunDiagnosis(requestDTO);

            if (result.IsFailure) return BadRequest(result.Value);

            return Ok(result.Value);
        }

        public async Task<IActionResult> GetDiagnosis()
        {
            return Ok(await _patientService.GetDiagnosis());
        }

        public async Task<IActionResult> GetDiagnosisById(int diagnosisId)
        {
            return Ok(await _patientService.GetDiagnosisById(diagnosisId));
        }

        public async Task<IActionResult> GetConsultationIllnessList()
        {
            var result = await _patientService.GetConsultationIllnessList();

            return Ok(result);
        }

        public async Task<IActionResult> GetConsultationSymptomByIllnessId(int IllnessId)
        {
            var result = await _patientService.GetConsultationSymptomByIllnessId(IllnessId);

            if (result.IsFailure) return BadRequest(result.Value);

            return Ok(result.Value);
        }
    }
}
