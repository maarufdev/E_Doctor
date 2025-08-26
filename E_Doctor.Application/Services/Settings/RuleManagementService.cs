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
        public async Task<bool> DeleteIllnessById(int id)
        {
            var illness = await _context.Illnesses.FirstOrDefaultAsync(d => d.Id == id);

            if (illness is null) return false;

            illness.IsActive = false;
            illness.UpdatedOn = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<IllnessSummaryDTO>> GetIllnessList()
        {
            var result = await _context.Illnesses
                .AsNoTracking()
                .Include(d => d.Rules)
                .Where(d => d.IsActive)
                .OrderByDescending(d => d.UpdatedOn ?? d.CreatedOn)
                .Select(d => new IllnessSummaryDTO(
                        d.Id,
                        d.IllnessName,
                        d.Description ?? string.Empty,
                        d.Rules != null ? d.Rules.Count(x => x.IsActive) : 0
                    ))
                .ToListAsync();

            return result;
        }

        public async Task<IllnessDTO> GetIllnessById(int id)
        {
            var disease = await _context.Illnesses
                .AsNoTracking()
                .Include(d => d.Rules.Where(x => x.IsActive == true))
                .Where(x => x.Id == id && x.IsActive)
                .FirstOrDefaultAsync();

            var result = disease.ToDTO() as IllnessDTO;

            return result;
        }

        public async Task<bool> SaveIllness(IllnessDTO requestDto)
        {
            try
            {
                ValidateSaveIllnessRequestDto(requestDto);

                var illnessId = requestDto.IllnessId;

                if (illnessId == 0)
                {
                    var illnessEntity = new IllnessEntity
                    {
                        IllnessName = requestDto.IllnessName,
                        Description = requestDto.Description,
                        Rules = [],
                        IsActive = true,
                        CreatedOn = DateTime.UtcNow
                    };

                    foreach (var rule in requestDto.Rules)
                    {
                        var newRuleEntity = new IllnessRuleEntity
                        {
                            IllnessId = illnessEntity.Id,
                            SymptomId = rule.SymptomId,
                            Condition = rule.Condition,
                            Days = rule.Days,
                            IsActive = true,
                        };

                        illnessEntity.Rules.Add(newRuleEntity);
                    }

                    await _context.Illnesses.AddAsync(illnessEntity);
                }
                else
                {
                    var illness = await _context.Illnesses
                        .Include(x => x.Rules)
                        .Where(x => x.Id == requestDto.IllnessId && x.IsActive == true)
                        .FirstOrDefaultAsync();

                    if (illness == null) return false;

                    illness.IllnessName = requestDto.IllnessName;
                    illness.Description = requestDto.Description;
                    illness.UpdatedOn = DateTime.UtcNow;

                    var existingRules = illness.Rules?.ToList() ?? [];

                    var toRemoveRules = existingRules
                        .Where(x => !requestDto.Rules.Any(r => r.SymptomId == x.SymptomId && illnessId == x.IllnessId));

                    foreach (var rule in toRemoveRules)
                    {
                        rule.IsActive = false;
                    }

                    foreach (var rule in requestDto.Rules)
                    {
                        var existingRule = illness.Rules?.FirstOrDefault(r => r.SymptomId == rule.SymptomId);
                        
                        if (existingRule is not null)
                        {
                            existingRule.IsActive = true;
                            existingRule.Condition = rule.Condition;
                            existingRule.Days = rule.Days;
                        }
                        else
                        {
                            var newRule = new IllnessRuleEntity
                            {
                                SymptomId = rule.SymptomId,
                                IllnessId = illnessId,
                                Condition = rule.Condition,
                                Days = rule.Days,
                                IsActive = true
                            };

                            illness.Rules.Add(newRule);
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

        private static void ValidateSaveIllnessRequestDto(IllnessDTO requestDto)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(requestDto.IllnessName);
            ArgumentException.ThrowIfNullOrWhiteSpace(requestDto.Description);
            ArgumentNullException.ThrowIfNull(requestDto.Rules);
            ArgumentOutOfRangeException.ThrowIfEqual(0, requestDto.Rules.Count);
        }
    }
}
