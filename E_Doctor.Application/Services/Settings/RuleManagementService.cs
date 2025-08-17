using E_Doctor.Application.DTOs.Settings.RuleManagements;
using E_Doctor.Application.Interfaces.Features.Settings;
using E_Doctor.Core.Domain.Entities;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Application.Services.Settings
{
    internal class RuleManagementService(AppDbContext context) : IRuleManagementService
    {
        private readonly AppDbContext _context = context;
        public async Task<bool> DeleteDisease(int id)
        {
            var disease = await _context.Diseases.FirstOrDefaultAsync(d => d.Id == id);

            if (disease is null) return false;

            disease.IsActive = false;
            disease.UpdatedOn = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<DiseaseSummaryDTO>> GetDiseaseList()
        {
            var result = await _context.Diseases
                .AsNoTracking()
                .Include(d => d.Rules)
                .Where(d => d.IsActive)
                .Select(d => new DiseaseSummaryDTO(
                        d.Id,
                        d.DiseaseName,
                        d.Description ?? string.Empty,
                        d.Rules != null ? d.Rules.Count(x => x.IsActive) : 0
                    ))
                .ToListAsync();

            return result;
        }

        public async Task<DiseaseDTO> GetDiseaseRule(int id)
        {
            var disease = await _context.Diseases
                .AsNoTracking()
                .Include(d => d.Rules)
                .Where(x => x.Id == id && x.IsActive)
                .FirstOrDefaultAsync();

            return disease.ToDTO();
        }

        public async Task<bool> SaveDisease(DiseaseDTO requestDto)
        {
            if (requestDto.DiseaseId == 0)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(requestDto.DiseaseName);
                ArgumentException.ThrowIfNullOrWhiteSpace(requestDto.Description);

                var diseaseEntity = new DiseaseEntity
                {
                    DiseaseName = requestDto.DiseaseName,
                    Description = requestDto.Description,
                    Rules = [],
                    IsActive = true
                };

                foreach (var rule in requestDto.Rules)
                {
                    var newRuleEntity = new DiseaseRuleEntity
                    {
                        DiseaseId = diseaseEntity.Id,
                        SymptomId = rule.SymptomId,
                        Condition = rule.Condition,
                        Days = rule.Days,
                        IsActive = true,
                    };

                    diseaseEntity.Rules.Add(newRuleEntity);
                }

                await _context.Diseases.AddAsync(diseaseEntity);
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
