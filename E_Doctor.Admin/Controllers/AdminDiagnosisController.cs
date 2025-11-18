using E_Doctor.Application.Constants;
using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Diagnosis;
using E_Doctor.Application.Interfaces.Features.Diagnosis;
using E_Doctor.Infrastructure.Constants;
using E_Doctor.Infrastructure.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Web.Controllers
{
    [Authorize(Roles = RoleConstants.Admin)]
    public class AdminDiagnosisController : Controller
    {
        private readonly IDiagnosisService _diagnosisService;
        public AdminDiagnosisController(IDiagnosisService diagnosisService)
        {
            _diagnosisService = diagnosisService;
        }
        public IActionResult Index(AdminDiagnosisTab? tabName = AdminDiagnosisTab.Diagnosis)
        {
            ViewBag.ActiveTab = tabName;
            var pageTitle = "Diagnosis";

            switch (tabName)
            {
                case AdminDiagnosisTab.Symptom:
                    pageTitle = "Manage Symptoms";
                    break;
                case AdminDiagnosisTab.Illness:
                    pageTitle = "Manage Illnesses";
                    break;
                case AdminDiagnosisTab.ManageUser:
                    pageTitle = "Manage Users";
                    break;
                default:
                    pageTitle = "Diagnosis";
                    break;
            }

            ViewBag.PageTitle = pageTitle;

            return View();
        }

        public async Task<IActionResult> GetDiagnosis(GetDiagnosisParamsDTO getDiagnosisParams)
        {
            return Ok(await  _diagnosisService.GetDiagnosis(getDiagnosisParams));
        }

        public async Task<IActionResult> RunDiagnosis([FromBody] RunDiagnosisDTO requestDTO)
        {
            if (requestDTO is null) return BadRequest("Please provide diagnosis");

            var result = await _diagnosisService.RunDiagnosis(requestDTO);

            if (result.IsFailure) return BadRequest(result.Value);

            return Ok(result.Value);
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

        public async Task<IActionResult> GetPhysicalExamItems()
        {
            var result = await _diagnosisService.GetPhysicalItems();
            
            if (result.IsFailure) return BadRequest(result.Error);
            
            return Ok(result.Value);
        }
    }
}