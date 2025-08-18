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
                .OrderByDescending(d => d.UpdatedOn ?? d.CreatedOn)
                .Select(d => new DiseaseSummaryDTO(
                        d.Id,
                        d.DiseaseName,
                        d.Description ?? string.Empty,
                        d.Rules != null ? d.Rules.Count(x => x.IsActive) : 0
                    ))
                .ToListAsync();

            return result;
        }

        public async Task<DiseaseDTO> GetDiseaseById(int id)
        {
            var disease = await _context.Diseases
                .AsNoTracking()
                .Include(d => d.Rules.Where(x => x.IsActive == true))
                .Where(x => x.Id == id && x.IsActive)
                .FirstOrDefaultAsync();

            var result = disease.ToDTO() as DiseaseDTO;

            return result;
        }

        public async Task<bool> SaveDisease(DiseaseDTO requestDto)
        {
            try
            {
                ValidateSaveDiseaseRequestDto(requestDto);

                var diseaseId = requestDto.DiseaseId;

                if (diseaseId == 0)
                {
                    var diseaseEntity = new DiseaseEntity
                    {
                        DiseaseName = requestDto.DiseaseName,
                        Description = requestDto.Description,
                        Rules = [],
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow
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
                else
                {
                    var disease = await _context.Diseases
                        .Include(x => x.Rules)
                        .Where(x => x.Id == requestDto.DiseaseId && x.IsActive == true)
                        .FirstOrDefaultAsync();

                    if (disease == null) return false;

                    disease.DiseaseName = requestDto.DiseaseName;
                    disease.Description = requestDto.Description;
                    disease.UpdatedOn = DateTime.UtcNow;

                    var existingRules = disease.Rules?.ToList() ?? [];

                    var toRemoveRules = existingRules
                        .Where(x => !requestDto.Rules.Any(r => r.SymptomId == x.SymptomId && diseaseId == x.DiseaseId));

                    foreach (var rule in toRemoveRules)
                    {
                        rule.IsActive = false;
                    }

                    foreach (var rule in requestDto.Rules)
                    {
                        var existingRule = disease.Rules?.FirstOrDefault(r => r.SymptomId == rule.SymptomId);
                        
                        if (existingRule is not null)
                        {
                            existingRule.IsActive = true;
                            existingRule.Condition = rule.Condition;
                            existingRule.Days = rule.Days;
                        }
                        else
                        {
                            var newRule = new DiseaseRuleEntity
                            {
                                SymptomId = rule.SymptomId,
                                DiseaseId = diseaseId,
                                Condition = rule.Condition,
                                Days = rule.Days,
                                IsActive = true
                            };

                            disease.Rules.Add(newRule);
                        }
                    }
                }

                var result = await _context.SaveChangesAsync() > 0;

                return result;
            }
            catch(DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            catch(DbUpdateException dbEx)
            {
                Console.WriteLine(dbEx.ToString());
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        private static void ValidateSaveDiseaseRequestDto(DiseaseDTO requestDto)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(requestDto.DiseaseName);
            ArgumentException.ThrowIfNullOrWhiteSpace(requestDto.Description);
            ArgumentNullException.ThrowIfNull(requestDto.Rules);
            ArgumentOutOfRangeException.ThrowIfEqual(0, requestDto.Rules.Count);
        }
    }
}
