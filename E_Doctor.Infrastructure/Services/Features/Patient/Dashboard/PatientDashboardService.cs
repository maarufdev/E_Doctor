using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Dashboard;
using E_Doctor.Application.Helpers;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Application.Interfaces.Features.Patient.Dashboard;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Infrastructure.Services.Features.Patient.Dashboard;
internal class PatientDashboardService : IPatientDashboardService
{
    private readonly AppDbContext _dbContext;
    private readonly IUserManagerService _userServices;
    public PatientDashboardService(AppDbContext dbContext, IUserManagerService userManager)
    {
        _dbContext = dbContext;
        _userServices = userManager;
    }

    public async Task<Result<GetDashboardDetailsDTO>> GetDashboardDetails()
    {
        var userId = await _userServices.GetUserId();

        var cardDetails = GetDashboardCardDetailsDTO.Create(
            0,0,
                1
        );

        var recentDiagnosis = await _dbContext.Diagnosis
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
