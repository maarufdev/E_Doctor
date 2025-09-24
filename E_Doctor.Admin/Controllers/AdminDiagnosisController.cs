using E_Doctor.Application.Constants;
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
        public IActionResult Index(AdminDiagnosisTab? tabName = AdminDiagnosisTab.Diagnosis)
        {
            ViewBag.ActiveTab = tabName;
            var pageTitle = "Diagnosis";

            switch (tabName)
            {
                case AdminDiagnosisTab.Symptom:
                    pageTitle = "Manage Symptoms";
                    break;
                case AdminDiagnosisTab.Consultation:
                    pageTitle = "Consulation";
                    break;
                case AdminDiagnosisTab.Illness:
                    pageTitle = "Manage Illnesses";
                    break;
                default:
                    pageTitle = "Diagnosis";
                    break;
            }

            ViewBag.PageTitle = pageTitle;

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

        public async Task<IActionResult> GetDiagnosisById(int diagnosisId)
        {
            return Ok(await _diagnosisService.GetDiagnosisById(diagnosisId));
        }
    }
}