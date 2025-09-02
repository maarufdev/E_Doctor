using E_Doctor.Application.DTOs.Common.ExportIllnessDTOs;
using E_Doctor.Application.DTOs.Diagnosis;
using E_Doctor.Application.DTOs.Settings.Symptoms;
using E_Doctor.Application.Interfaces.Features.Patient.Diagnosis;
using E_Doctor.Core.Domain.Entities.Patient;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace E_Doctor.Application.Services.Patient.Diagnosis
{
    internal class PatientService : IPatientService
    {
        private readonly PatientAppDbContext _context;
        public PatientService(PatientAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DiagnosisListDTO>> GetDiagnosis()
        {
            var diagnosis = await _context.PatientDiagnosis
                .Where(d => d.IsActive)
                .OrderByDescending(d => d.UpdatedOn ?? d.CreatedOn)
                .Select(d => new DiagnosisListDTO(
                    d.Id,
                    d.UpdatedOn ?? d.CreatedOn,
                    d.Symptoms,
                    d.IllnessName
                 ))
                .ToListAsync();

            return diagnosis;
        }

        public async Task<IEnumerable<GetSymptomDTO>> GetSymtoms()
        {
            return await _context.PatientSymptoms
                .Select(s => new GetSymptomDTO(s.SymptomId, s.SymptomName)).ToListAsync();
        }

        public async Task<bool> MigrateRules(string jsonData)
        {
            try
            {
                var config = JsonSerializer.Deserialize<ExportIllnessConfigDTO>(jsonData);

                if (config is null) return false;

                await _context.Database.ExecuteSqlRawAsync("DELETE FROM PatientSymptoms");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM PatientIllnesses");
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM PatientIllnesRules");

                var newSymptoms = config.Symptoms
                    .Select(s => new PatientSymptomEntity
                    {
                        SymptomId = s.SymptomId,
                        SymptomName = s.SymptomName,
                        UpdatedOn = DateTime.UtcNow,
                    });

                await _context.PatientSymptoms.AddRangeAsync(newSymptoms);

                var newIllnesses = config.Illnesses
                    .Select(i => new PatientIllnessEntity
                    {
                        IllnessId = i.IllnessId,
                        IllnessName = i.IllnessName,
                        Description = i.Description,
                        UpdatedOn = DateTime.UtcNow
                    });

                await _context.PatientIllnesses.AddRangeAsync(newIllnesses);

                var newRules = config.Rules
                    .Select(r => new PatientRulesEntity
                    {
                        SymptomId = r.SymptomId,
                        IllnessId = r.IllnessId,
                        Condition = r.Condition,
                        Days = r.Days,
                        Weight = r.Weight,
                    });

                await _context.PatientIllnesRules.AddRangeAsync(newRules);

                var result = await _context.SaveChangesAsync() > 0;

                return result;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public Task<List<DiagnosisResultDTO>> RunDiagnosis(List<RunDiagnosisDTO> diagnosisRequest)
        {
            return Task.FromResult(new List<DiagnosisResultDTO>());
        }
    }
}
