using E_Doctor.Core.Domain.Entities.Admin;

namespace E_Doctor.Application.DTOs.Settings.RuleManagements
{
    public static class RuleManagementDTOMapper
    {
        public static IllnessDTO? ToDTO(this IllnessEntity? entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var rulesDTO = new List<IllnessRuleDTO>();

            if (entity.Rules?.Count > 0)
            {
                rulesDTO = entity.Rules
                .Where(s => s.Symptom != null && s.Symptom.IsActive)
                .Select(r => new IllnessRuleDTO(
                    r.SymptomId,
                    r.Condition,
                    r.Days,
                    r.Weight
                ))
                .ToList();
            }

            var illnessDTO = new IllnessDTO(
                entity.Id,
                entity.IllnessName,
                entity.Description ?? string.Empty,
                rulesDTO
                );

            return illnessDTO;
        }
    }
}
