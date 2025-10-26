using E_Doctor.Application.DTOs.Common;
using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Settings.Symptoms;
using E_Doctor.Application.Interfaces.Features.Admin.Settings;
using E_Doctor.Application.Interfaces.Features.Common;
using E_Doctor.Core.Constants.Enums;
using E_Doctor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Infrastructure.Services.Features.Admin.Settings
{
    internal class SymptomService(
        AppDbContext context, 
        IActivityLoggerService activityService,
        IUserManagerService userManagerService) : ISymptomService
    {
        private readonly AppDbContext _context = context;
        private readonly IActivityLoggerService _activityLoggerService = activityService;
        private readonly IUserManagerService _userManagerService = userManagerService;

        public async Task<GetSymptomDTO?> GetSymptomById(int symptomId)
        {
            var symptom = await _context.Symptoms
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == symptomId);

            if(symptom is null)
            {
                ArgumentNullException.ThrowIfNull(symptom);
            }

            var symptomDTO = new GetSymptomDTO(symptom.Id, symptom.Name, symptom.QuestionText);

            return symptomDTO;

        }

        public async Task<PagedResult<GetSymptomDTO>> GetSymptoms(GetSymptomsRequestDTO requestDTO)
        {
            var query = _context.Symptoms.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(requestDTO.SearchText))
            {
                query = query.Where(x => x.Name.ToLower().Contains(requestDTO.SearchText.ToLower()));
            }

            query = query
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.UpdatedOn ?? x.CreatedOn);

            var totalCount = await query.CountAsync();

            var symptoms = await query
                .Skip((requestDTO.PageNumber - 1) * requestDTO.PageSize)
                .Take(requestDTO.PageSize)
                .Select(x => new GetSymptomDTO(x.Id, x.Name, x.QuestionText))
                .ToListAsync();
            

            return new PagedResult<GetSymptomDTO>
            {
                Items = symptoms,
                TotalCount = totalCount,
                PageSize = requestDTO.PageSize,
                PageNumber = requestDTO.PageNumber,
            };
        }

        public async Task<bool> RemoveSymptom(int symptomId)
        {
            var entity = await _context.Symptoms.FirstOrDefaultAsync(x => x.Id == symptomId);

            if (entity is null) return false;

            entity.IsActive = false;
            entity.UpdatedOn = DateTime.UtcNow;

            var userId = await _userManagerService.GetUserId();

            await _activityLoggerService.LogAsync(
                userId,
                UserActivityTypes.RemoveSymptom,
                entity.Name
                );

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Result> SaveSymptom(SaveSymptomDTO saveSymptomDto)
        {
            if(saveSymptomDto == null)
            {
                ArgumentNullException.ThrowIfNull(saveSymptomDto);
            }

            string activityDescription = string.Empty;
            UserActivityTypes activityType;

            var userId = await _userManagerService.GetUserId();

            if(saveSymptomDto.SymptomId == 0)
            {
                var isExist = await _context.Symptoms.AnyAsync(s => s.Name.ToLower() == saveSymptomDto.SymptomName.ToLower() && s.IsActive);
                if (isExist) return Result.Failure($"{saveSymptomDto.SymptomName} is already exists.");

                var entity = saveSymptomDto.CreateSymptomEntity();
                
                await _context.Symptoms.AddAsync(entity);
                activityDescription = entity.Name;
                activityType = UserActivityTypes.CreateSymptom;
            } 
            else
            {
                var toUpdate = await _context.Symptoms.FirstOrDefaultAsync(x => x.Id == saveSymptomDto.SymptomId && x.IsActive);

                if(toUpdate is null)
                {
                    ArgumentNullException.ThrowIfNull(toUpdate);
                }

                toUpdate = saveSymptomDto.UpdateSymptomEntity(toUpdate);
                _context.Symptoms.Update(toUpdate);
                
                activityDescription = toUpdate.Name;
                activityType = UserActivityTypes.UpdateSymptom;
            }

            var result = await _context.SaveChangesAsync() > 0;

            if(!result) return Result.Failure("Something went wrong!");

            await _activityLoggerService.LogAsync(
                userId,
                activityType,
                activityDescription
                );

            return Result.Success();
        }
        
    }
}
