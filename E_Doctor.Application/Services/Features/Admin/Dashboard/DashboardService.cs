using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Dashboard;
using E_Doctor.Application.Helpers;
using E_Doctor.Application.Interfaces.Data;
using E_Doctor.Application.Interfaces.Features.Admin.Dashboard;
using E_Doctor.Infrastructure.Constants;
using E_Doctor.Infrastructure.Data;
using E_Doctor.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Application.Services.Features.Admin.Dashboard;
internal class DashboardService : IDashboardService
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUserIdentity> _userManager;
    public DashboardService(AppDbContext dbContext, UserManager<AppUserIdentity> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
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
                .CountAsync(i => i.IsActive),
                (await _userManager.GetUsersInRoleAsync(RoleConstants.Patient)).Count
        );

        var recentDiagnosis = await _dbContext.Diagnosis
            .AsNoTracking()
            .Join(_dbContext.Users, d => d.UserId, u => u.Id, (Diagnosis, User) => new { User, Diagnosis })
            .Where(x => x.Diagnosis.IsActive && x.User.IsActive)
            .OrderByDescending(d => d.Diagnosis.UpdatedOn ?? d.Diagnosis.CreatedOn)
            .Select(d => new
            {
                d.Diagnosis.Id,
                UpdatedOn = d.Diagnosis.UpdatedOn ?? d.Diagnosis.CreatedOn,
                Symptoms = string.Join(", ", d.Diagnosis.DiagnosSymptoms.Select(s => $"{s.SymptomName}").ToList()),
                IllnessName = string.Join(", ", d.Diagnosis.DiagnosIllnesses.Select(i => $"{i.Illness} {i.Score.ToString("F2")}%").ToList()),
                UserName = d.User.UserName
            })
            .Take(5)
            .ToListAsync();

        var recentDiagnosisDTO = recentDiagnosis
            .Select(d =>
            {
                var dateTime = d.UpdatedOn;
                return new GetDashboardRecentDiagnosisDTO(DateTimeHelper.ToLocalShortDateTimeString(dateTime.Value), d.Symptoms, d.IllnessName, d.UserName);
            })
            .ToList();
        
        var result = GetDashboardDetailsDTO.Create(cardDetails, recentDiagnosisDTO);

        return Result<GetDashboardDetailsDTO>.Success(result);
    }
}
