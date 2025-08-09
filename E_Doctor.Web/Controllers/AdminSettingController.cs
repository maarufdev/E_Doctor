using E_Doctor.Application.DTOs.Settings.Symptoms;
using E_Doctor.Application.Interfaces.Features.Settings;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Web.Controllers
{
    public class AdminSettingController(ISymptomService symptomService) : Controller
    {
        private readonly ISymptomService _symptomService = symptomService;
        public IActionResult Index()
        {
            return View();
        }

        #region Symptoms Settings
        public async Task<IActionResult> GetSymptoms()
        {
            var symptoms = await _symptomService.GetSymptoms();

            if(symptoms is null) return BadRequest();

            return Ok(symptoms);
        }

        public async Task<IActionResult> GetSymptomById(int symptomId)
        {
            var symptom = await _symptomService.GetSymptomById(symptomId);

            if (symptom is null) return BadRequest();

            return Ok(symptom);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSymptom([FromBody] SaveSymptomDTO saveSymptomDto)
        {
            var isSuccess = await _symptomService.SaveSymptom(saveSymptomDto);

            if (!isSuccess) return BadRequest();

            return Ok(isSuccess);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveSymptom(int symptomId)
        {
            var isSuccess = await _symptomService.RemoveSymptom(symptomId);

            if (!isSuccess) return BadRequest();

            return Ok(isSuccess);
        }
        
        #endregion
    }
}
