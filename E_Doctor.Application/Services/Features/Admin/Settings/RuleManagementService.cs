using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Common.ExportIllnessDTOs;
using E_Doctor.Application.DTOs.Settings.RuleManagements;
using E_Doctor.Application.DTOs.Settings.Symptoms;
using E_Doctor.Application.Interfaces.Features.Admin.Settings;
using E_Doctor.Core.Domain.Entities.Admin;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace E_Doctor.Application.Services.Features.Admin.Settings
{
    internal class RuleManagementService(AppDbContext context) : IRuleManagementService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<GetSymptomDTO>> GetSymptoms()
        {
            var symptoms = await _context.Symptoms
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn)
                .Select(x => new GetSymptomDTO(x.Id, x.Name, x.QuestionText))
                .ToListAsync();

            return symptoms;
        }

        public async Task<bool> DeleteIllnessById(int id)
        {
            var illness = await _context.Illnesses.FirstOrDefaultAsync(d => d.Id == id);

            if (illness is null) return false;

            illness.IsActive = false;
            illness.UpdatedOn = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PagedResult<IllnessSummaryDTO>> GetIllnessList(GetIllnessRequestDTO requestParam)
        {
            var query = _context.Illnesses
                .AsNoTracking()
                .Where(i => i.IsActive)
                .OrderByDescending(i => i.UpdatedOn ?? i.CreatedOn)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(requestParam.SearchText))
            {
                var searchText = requestParam.SearchText.ToLower();

                query = query
                    .Where(i =>
                        i.IllnessName.ToLower().Contains(searchText) ||
                        i.Description.ToLower().Contains(searchText)
                    );
            }

            var totalCounts = await query.CountAsync();

            var illnesses = await query
                .Skip((requestParam.PageNumber - 1) * requestParam.PageSize)
                .Take(requestParam.PageSize)
                .Select(d => new IllnessSummaryDTO(
                        d.Id,
                        d.IllnessName,
                        d.Description ?? string.Empty,
                        d.Rules != null ? d.Rules.Count(x => x.IsActive && x.Symptom != null && x.Symptom.IsActive) : 0
                    ))
                .ToListAsync();

            return new PagedResult<IllnessSummaryDTO>
            {
                Items = illnesses,
                TotalCount = totalCounts,
                PageSize = requestParam.PageSize,
                PageNumber = requestParam.PageNumber,
            };
        }

        public async Task<IllnessDTO> GetIllnessById(int id)
        {
            var disease = await _context.Illnesses
                .AsNoTracking()
                .Include(d => d.Rules)
                    .ThenInclude(r => r.Symptom)
                .Where(x => x.Id == id && x.IsActive)
                .FirstOrDefaultAsync();

            var result = disease.ToDTO() as IllnessDTO;

            return result;
        }

        public async Task<Result> SaveIllness(IllnessDTO requestDto)
        {
            try
            {
                ValidateSaveIllnessRequestDto(requestDto);

                var illnessId = requestDto.IllnessId;

                if (illnessId == 0)
                {
                    var isExist = await _context.Illnesses
                        .AnyAsync(i => i.IllnessName.ToLower() == requestDto.IllnessName.ToLower());

                    if (isExist) return Result.Failure($"{requestDto.IllnessName} is already exists.");

                    var illnessEntity = new IllnessEntity
                    {
                        IllnessName = requestDto.IllnessName,
                        Description = requestDto.Description,
                        Prescription = requestDto.Prescription,
                        Notes = requestDto.Notes,
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

                    if (illness == null) return Result.Failure("Illness to be updated does not exist.");

                    illness.IllnessName = requestDto.IllnessName;
                    illness.Description = requestDto.Description;
                    illness.Prescription = requestDto.Prescription;
                    illness.Notes = requestDto.Notes;
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
                        }
                        else
                        {
                            var newRule = new IllnessRuleEntity
                            {
                                SymptomId = rule.SymptomId,
                                IllnessId = illnessId,
                                IsActive = true
                            };

                            illness.Rules.Add(newRule);
                        }
                    }
                }

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result.Failure("Something went wrong!");

                return Result.Success();
            }
            catch(DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.ToString());
                return Result.Failure("Something went wrong!");
            }
            catch(DbUpdateException dbEx)
            {
                Console.WriteLine(dbEx.ToString());
                return Result.Failure("Something went wrong!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Result.Failure("Something went wrong!");
            }
        }

        private static void ValidateSaveIllnessRequestDto(IllnessDTO requestDto)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(requestDto.IllnessName);
            ArgumentException.ThrowIfNullOrWhiteSpace(requestDto.Description);
            ArgumentNullException.ThrowIfNull(requestDto.Rules);
            ArgumentOutOfRangeException.ThrowIfEqual(0, requestDto.Rules.Count);
        }

        public async Task<byte[]> ExportRulesConfigration()
        {
            try
            {
                var symptoms = await _context.Symptoms
                .AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .Select(s => new ExportSymptomDTO(s.Id, s.Name, s.QuestionText))
                .ToListAsync();

                var illnesses = await _context.Illnesses
                    .AsNoTracking()
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.IllnessName)
                    .Select(s => new ExportIllnessDTO(
                        s.Id, 
                        s.IllnessName, 
                        s.Description ?? string.Empty, 
                        s.Prescription ?? string.Empty,
                        s.Notes ?? string.Empty)
                    )
                    .ToListAsync();

                var rules = await _context.IllnessRules
                    .AsNoTracking()
                    .Include(r => r.Symptom)
                    .Include(r => r.Illness)
                    .Where(s => s.IsActive && s.Symptom.IsActive && s.Illness.IsActive)
                    .Select(r => new ExportRulesDTO(
                        r.SymptomId,
                        r.IllnessId
                        )
                    )
                    .ToListAsync();

                var result = new ExportIllnessConfigDTO
                {
                    Symptoms = symptoms,
                    Illnesses = illnesses,
                    Rules = rules
                };

                var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
                var jsonBytes = Encoding.UTF8.GetBytes(json);

                return jsonBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
