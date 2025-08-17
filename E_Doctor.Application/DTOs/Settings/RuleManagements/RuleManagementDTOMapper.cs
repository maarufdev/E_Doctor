using E_Doctor.Core.Domain.Entities;

namespace E_Doctor.Application.DTOs.Settings.RuleManagements
{
    public static class RuleManagementDTOMapper
    {
        public static DiseaseDTO ToDTO(this DiseaseEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var rulesDTO = new List<DiseaseRuleDTO>();

            if (entity.Rules?.Count > 0)
            {
                rulesDTO = entity.Rules
                .Select(r => new DiseaseRuleDTO(
                    r.SymptomId,
                    r.Condition,
                    r.Days
                ))
                .ToList();
            }

            var diseaseDTO = new DiseaseDTO(
                entity.Id,
                entity.DiseaseName,
                entity.Description ?? string.Empty,
                rulesDTO
                );

            return diseaseDTO;
        }
    }
}
