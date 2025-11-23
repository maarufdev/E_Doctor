using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Diagnosis;
using E_Doctor.Application.Interfaces.Features.Admin.Settings;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Application.Interfaces.Features.Diagnosis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Patient.Controllers
{
    [Authorize]
    public class DiagnosisController : Controller
    {
        private readonly IRuleManagementService _ruleManager;
        private readonly IDiagnosisService _diagnosisService;
        private readonly IUserManagerService _userManagerService;
        public DiagnosisController(
            IDiagnosisService patientService, 
            IRuleManagementService ruleManager,
            IUserManagerService userManagerService
            )
        {
            _diagnosisService = patientService;
            _ruleManager = ruleManager;
            _userManagerService = userManagerService;
        }
        public async Task<IActionResult> Index()
        {
            var pageTitle = "Consultation History";
            ViewBag.PageTitle = pageTitle;

            await _userManagerService.UpdateUserLoginDate();
            return View();
        }

        //[HttpPost]
        //public async Task<IActionResult> ImportRulesConfiguration([FromForm] IFormFile file)
        //{
        //    if (file is null) return BadRequest();

        //    string jsonContent;
            
        //    using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
        //    jsonContent = await reader.ReadToEndAsync();

        //    if(string.IsNullOrWhiteSpace(jsonContent)) return BadRequest();

        //    var result = await _diagnosisService.MigrateRules(jsonContent);

        //    return Ok(result);
        //}

        public async Task<IActionResult> GetSymptoms()
        {
            return Ok(await _ruleManager.GetSymptoms());
        }

        [HttpPost]
        public async Task<IActionResult> RunDiagnosis([FromBody] RunDiagnosisDTO requestDTO)
        {
            if (requestDTO is null) return BadRequest("Please provide diagnosis");

            var result = await _diagnosisService.RunDiagnosis(requestDTO);

            if (result.IsFailure) return BadRequest(result.Value);

            return Ok(result.Value);
        }

        public async Task<IActionResult> GetDiagnosis(GetDiagnosisParamsDTO getDiagnosisParams)
        {
            return Ok(await _diagnosisService.GetDiagnosis(getDiagnosisParams));
        }

        public async Task<IActionResult> GetDiagnosisById(int diagnosisId)
        {
            return Ok(await _diagnosisService.GetDiagnosisById(diagnosisId));
        }

        public async Task<IActionResult> GetConsultationIllnessList()
        {
            var result = await _diagnosisService.GetConsultationIllnessList();

            return Ok(result);
        }

        public async Task<IActionResult> GetConsultationSymptomByIllnessId(int IllnessId)
        {
            var result = await _diagnosisService.GetConsultationSymptomByIllnessId(IllnessId);

            if (result.IsFailure) return BadRequest(result.Value);

            return Ok(result.Value);
        }

        public async Task<IActionResult> DeleteDiagnosisById(int DiagnosisId)
        {
            var result = await _diagnosisService.DeleteDiagnosisById(DiagnosisId);

            if (result.IsFailure) return BadRequest(result.IsFailure);

            return Ok(result.IsSuccess);
        }

        public async Task<IActionResult> GetPhysicalExamById(int physicalExamId)
        {
            if (physicalExamId == 0) return BadRequest("Physical Exam Report is not valid.");

            var result = await _diagnosisService.GetPhysicalExamById(physicalExamId);

            if (result.IsFailure) return BadRequest(result.Error);

            return Ok(result.Value);
        }

        public async Task<IActionResult> GetPhysicalExamItems()
        {
            var result = await _diagnosisService.GetPhysicalItems();

            if (result.IsFailure) return BadRequest(result.Error);

            return Ok(result.Value);
        }
    }
}
