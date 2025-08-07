using E_Doctor.Application.DTOs.Settings.Symptoms;
using E_Doctor.Application.Interfaces.Features.Settings;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Application.Services.Settings
{
    internal class SymptomService(AppDbContext context) : ISymptomService
    {
        private readonly AppDbContext _context = context;

        public async Task<GetSymptomDTO?> GetSymptomById(int symptomId)
        {
            var symptom = await _context.Symptoms
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == symptomId);

            if(symptom is null)
            {
                ArgumentNullException.ThrowIfNull(symptom);
            }

            var symptomDTO = new GetSymptomDTO(symptom.Id, symptom.Name);

            return symptomDTO;

        }

        public async Task<IEnumerable<GetSymptomDTO>> GetSymptoms()
        {
            var symptoms = await _context.Symptoms
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn)
                .Select(x => new GetSymptomDTO(x.Id, x.Name))
                .ToListAsync();

            return symptoms;
        }

        public async Task<bool> RemoveSymptom(int symptomId)
        {
            var entity = await _context.Symptoms.FirstOrDefaultAsync(x => x.Id == symptomId);

            if (entity is null) return false;

            entity.IsActive = false;
            entity.UpdatedOn = DateTime.UtcNow;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SaveSymptom(SaveSymptomDTO saveSymptomDto)
        {
            if(saveSymptomDto == null)
            {
                ArgumentNullException.ThrowIfNull(saveSymptomDto);
            }

            if(saveSymptomDto.SymptomId == 0)
            {
                var entity = saveSymptomDto.ToCreateEntity();
                
                await _context.Symptoms.AddAsync(entity);
            } 
            else
            {
                var toUpdate = await _context.Symptoms.FirstOrDefaultAsync(x => x.Id == saveSymptomDto.SymptomId && x.IsActive);

                if(toUpdate is null)
                {
                    ArgumentNullException.ThrowIfNull(toUpdate);
                }

                toUpdate = saveSymptomDto.ToUpdateEntity(toUpdate);

                _context.Symptoms.Update(toUpdate);
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
