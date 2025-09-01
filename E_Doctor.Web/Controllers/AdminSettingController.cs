using E_Doctor.Application.Constants;
using E_Doctor.Application.DTOs.Settings.RuleManagements;
using E_Doctor.Application.DTOs.Settings.Symptoms;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Application.Interfaces.Features.Settings;
using Microsoft.AspNetCore.Mvc;

namespace E_Doctor.Web.Controllers
{
    public class AdminSettingController : Controller
    {
        private readonly ISymptomService _symptomService;
        private readonly IRuleManagementService _ruleManager;
        private readonly ICommonService _commonService;
        public AdminSettingController(
            ISymptomService symptomService,
            IRuleManagementService ruleManager,
            ICommonService commonService
            )
        {
            _symptomService = symptomService;
            _ruleManager = ruleManager;
            _commonService = commonService;
        }

        public IActionResult Index(SettingTab? tabName = SettingTab.Symptom)
        {
            ViewBag.ActiveTab = tabName;

            return View();
        }

        #region common
        public IActionResult GetRuleConditions()
        {
            return Ok(_commonService.GetRuleConditions());
        }
        
        public IActionResult GetWeightRules()
        {
            return Ok(_commonService.GetWeightRules());
        }

        #endregion

        #region Symptoms Settings
        public async Task<IActionResult> GetSymptoms()
        {
            var symptoms = await _symptomService.GetSymptoms();

            if (symptoms is null) return BadRequest();

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

        #region Illness Rule Management
        public async Task<IActionResult> GetIllnessList()
        {
            return Ok(await _ruleManager.GetIllnessList());
        }

        public async Task<IActionResult> GetIllnessById(int id)
        {
            if (id == 0) return BadRequest();

            var result = await _ruleManager.GetIllnessById(id);

            if (result == null) return NotFound();

            return Ok(result);

        }

        [HttpPost]
        public async Task<IActionResult> SaveIllness([FromBody] IllnessDTO diseaseDTO)
        {
            if (diseaseDTO == null) return BadRequest();

            var result = await _ruleManager.SaveIllness(diseaseDTO);

            if (!result) return BadRequest();

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteIllnessById(int id)
        {
            if(id == 0) return BadRequest();

            var result = await _ruleManager.DeleteIllnessById(id);
            
            if (!result) return BadRequest();

            return Ok(result);
        }

        public async Task<IActionResult> GetExportRulesConfiguration()
        {
            var result = await _ruleManager.ExportRulesConfigration();

            return File(result, "application/json", "rules-config.json");
        }

        #endregion
    }
}
