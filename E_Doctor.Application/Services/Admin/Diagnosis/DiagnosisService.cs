using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Diagnosis;
using E_Doctor.Application.Interfaces.Features.Admin.Diagnosis;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Core.Domain.Entities.Admin;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Application.Services.Admin.Diagnosis;

internal class DiagnosisService : IDiagnosisService
{
    private readonly AdminAppDbContext _appDbContext;
    private readonly IUserManagerService _userManager;
    public DiagnosisService(AdminAppDbContext appDbContext, IUserManagerService userManager)
    {
        _appDbContext = appDbContext;
        _userManager = userManager;
    }

    public async Task<List<DiagnosisListDTO>> GetDiagnosis()
    {
        var diagnosis = await _appDbContext.DiagnosisTest
            .Where(d => d.IsActive)
            .OrderByDescending(d => d.UpdatedOn ?? d.CreatedOn)
            .Select(d => new DiagnosisListDTO(
                d.Id,
                d.UpdatedOn ?? d.CreatedOn,
                d.Symptoms,
                d.DiagnosisResult,
                d.Prescription
             ))
            .ToListAsync();

        return diagnosis;
    }


    public async Task<DiagnosisDetailsDTO> GetDiagnosisById(int diagnosisId)
    {
        var result = new DiagnosisDetailsDTO();

        var diagnosis = await _appDbContext.DiagnosisTest
            .AsNoTracking()
            .Where(d => d.IsActive && d.Id == diagnosisId)
            .FirstOrDefaultAsync();

        if (diagnosis == null) return result;

        return DiagnosisDetailsDTO.Create(
            diagnosis.DiagnosisResult ?? string.Empty, 
            diagnosis.Description ?? string.Empty, 
            diagnosis.Prescription ?? string.Empty,
            diagnosis.Notes ?? string.Empty);
    }
    public async Task<Result<DiagnosisDetailsDTO>> RunDiagnosis(RunDiagnosisDTO diagnosisRequest)
    {
        try
        {
            var currentUserId = await _userManager.GetUserId();
            var result = new List<DiagnosisResultDTO>();
            var diagnosisResult = new DiagnosisDetailsDTO();

            var illness = await _appDbContext.Illnesses
                .AsNoTracking()
                .Include(r => r.Rules)
                .ThenInclude(s => s.Symptom)
                .Where(i => i.IsActive == true && i.Id == diagnosisRequest.IllnessId)
                .Select(i => new
                {
                    IllnessId = i.Id,
                    i.IllnessName,
                    i.Prescription,
                    i.Description,
                    i.Notes,
                    Rules = i.Rules.Where(r => r.IsActive).ToList(),
                    RuleCount = i.Rules.Count(r => r.IsActive),
                    MatchedRules = i.Rules.Where(r => r.IsActive && diagnosisRequest.SymptomIds.Contains(r.SymptomId)).ToList(),
                    MatchedRuleCount = i.Rules.Count(r => r.IsActive && diagnosisRequest.SymptomIds.Contains(r.SymptomId)),
                })
                .FirstOrDefaultAsync();

            if (illness is null) return Result<DiagnosisDetailsDTO>.Failure("Illness cannot be found.");

            var score = illness.RuleCount / illness.MatchedRuleCount * 100;

            var patientDiagnosed = new DiagnosisTestEntity
            {
                UserId = currentUserId ?? 0,
                DiagnosisResult = string.Join(", ", $"{illness.IllnessName} {score.ToString("F2")}%"),
                Symptoms = string.Join(", ", illness.MatchedRules.Select(r => r.Symptom.Name)),
                Prescription = illness.Prescription,
                Description = illness.Description,
                Notes = illness.Notes,
                CreatedOn = DateTime.UtcNow,
                IsActive = true,
            };

            await _appDbContext.DiagnosisTest.AddAsync(patientDiagnosed);
            await _appDbContext.SaveChangesAsync();

            diagnosisResult = DiagnosisDetailsDTO.Create(
                patientDiagnosed.DiagnosisResult,
                patientDiagnosed.Description ?? string.Empty,
                patientDiagnosed.Prescription ?? string.Empty,
                patientDiagnosed.Notes ?? string.Empty);

            return Result<DiagnosisDetailsDTO>.Success(diagnosisResult);
        }
        catch (Exception ex) 
        { 
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    public async Task<List<GetConsultationIllnessDTO>> GetConsultationIllnessList()
    {
        return await _appDbContext.Illnesses
            .AsNoTracking()
            .Where(i => i.IsActive)
            .Select(i => new GetConsultationIllnessDTO(i.Id, i.IllnessName))
            .ToListAsync();
    }

    public async Task<Result<List<GetConsultationSymptomByIllnessIdDTO>>> GetConsultationSymptomByIllnessId(int illnessId)
    {
        var illness = await _appDbContext.Illnesses
            .AsNoTracking()
            .Include(r => r.Rules)
            .ThenInclude(s => s.Symptom)
            .Where(i => i.IsActive && i.Id == illnessId)
            .Select(i => new
            {
                IllnessId = i.Id,
                Symptoms = i.Rules.Where(r => r.IsActive).Select(s => s.Symptom).Where(s => s.IsActive).ToList()
            })
            .FirstOrDefaultAsync();

        if (illness == null) return Result<List<GetConsultationSymptomByIllnessIdDTO>>.Failure("Illness Symptoms available.");

        var symptoms = illness.Symptoms
            .Select(s => new GetConsultationSymptomByIllnessIdDTO(s.Id, s.Name))
            .ToList();

        return Result<List<GetConsultationSymptomByIllnessIdDTO>>.Success(symptoms);
    }
}