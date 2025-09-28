using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Dashboard;
using E_Doctor.Application.Helpers;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Application.Interfaces.Features.Patient.Dashboard;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Application.Services.Patient.Dashboard;
internal class PatientDashboardService : IPatientDashboardService
{
    private readonly PatientAppDbContext _dbContext;
    private readonly IUserManagerService _userServices;
    public PatientDashboardService(PatientAppDbContext dbContext, IUserManagerService userManager)
    {
        _dbContext = dbContext;
        _userServices = userManager;
    }

    public async Task<Result<GetDashboardDetailsDTO>> GetDashboardDetails()
    {
        var userId = await _userServices.GetUserId();

        var cardDetails = GetDashboardCardDetailsDTO.Create(
                await _dbContext.PatientSymptoms.CountAsync(),
                await _dbContext.PatientIllnesses.CountAsync(),
                await _dbContext.PatientIllnesRules
                .Include(r => r.Illness)
                .Include(r => r.Symptom)
                .CountAsync()
        );

        var recentDiagnosis = await _dbContext.PatientDiagnosis
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(d => d.UpdatedOn ?? d.CreatedOn)
            .Select(d => new
            {
                d.Id,
                UpdatedOn = d.UpdatedOn ?? d.CreatedOn,
                Symptoms = string.Join(", ", d.DiagnosSymptoms.Select(s => $"{s.SymptomName}").ToList()),
                IllnessName = string.Join(", ", d.DiagnosIllnesses.Select(i => $"{i.Illness} {i.Score.ToString("F2")}%").ToList()),
            })
            .Take(5)
            .ToListAsync();

        var recentDiagnosisDTO = recentDiagnosis
            .Select(d =>
            {
                return new GetDashboardRecentDiagnosisDTO(DateTimeHelper.ToLocalShortDateTimeString(d.UpdatedOn.Value), d.Symptoms, d.IllnessName);
            })
            .ToList();

        var result = GetDashboardDetailsDTO.Create(cardDetails, recentDiagnosisDTO);

        return Result<GetDashboardDetailsDTO>.Success(result);
    }
}
