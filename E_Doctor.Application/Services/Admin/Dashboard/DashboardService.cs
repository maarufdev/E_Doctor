using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Dashboard;
using E_Doctor.Application.Helpers;
using E_Doctor.Application.Interfaces.Features.Admin.Dashboard;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Application.Services.Admin.Dashboard;
internal class DashboardService : IDashboardService
{
    private readonly AdminAppDbContext _dbContext;
    public DashboardService(AdminAppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<Result<GetDashboardDetailsDTO>> GetDashboardDetails()
    {
        var cardDetails = GetDashboardCardDetailsDTO.Create(
                await _dbContext.Symptoms.CountAsync(s => s.IsActive),
                await _dbContext.Illnesses.CountAsync(i => i.IsActive),
                await _dbContext.IllnessRules
                .Include(r => r.Illness)
                .Include(r => r.Symptom)
                .Where(c => c.Illness.IsActive && c.Symptom.IsActive)
                .CountAsync(i => i.IsActive)
        );

        var recentDiagnosis = await _dbContext.DiagnosisTest
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(d => d.UpdatedOn ?? d.CreatedOn)
            .Take(5)
            .ToListAsync();

        var recentDiagnosisDTO = recentDiagnosis
            .Select(d =>
            {
                var dateTime = d.UpdatedOn ?? d.CreatedOn;
                return new GetDashboardRecentDiagnosisDTO(DateTimeHelper.ToLocalShortDateTimeString(dateTime.Value), d.Symptoms, d.DiagnosisResult);
            })
            .ToList();
        
        var result = GetDashboardDetailsDTO.Create(cardDetails, recentDiagnosisDTO);

        return Result<GetDashboardDetailsDTO>.Success(result);
    }
}
