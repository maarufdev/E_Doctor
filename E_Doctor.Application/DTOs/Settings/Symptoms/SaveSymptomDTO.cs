using E_Doctor.Core.Domain.Entities;

namespace E_Doctor.Application.DTOs.Settings.Symptoms;
public sealed record SaveSymptomDTO(int SymptomId, string SymptomName);

public static class SaveSymptomMapper
{
    public static SymptomEntity ToCreateEntity(this SaveSymptomDTO saveSymptomDTO)
    {
        ArgumentNullException.ThrowIfNull(saveSymptomDTO);

        var entity = new SymptomEntity
        {
            Name = saveSymptomDTO.SymptomName,
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
        };

        return entity;
    }


    public static SymptomEntity ToUpdateEntity(this SaveSymptomDTO saveSymptomDTO, SymptomEntity entity)
    {
        ArgumentNullException.ThrowIfNull(saveSymptomDTO);
        ArgumentOutOfRangeException.ThrowIfEqual(saveSymptomDTO.SymptomId, 0);
        ArgumentException.ThrowIfNullOrWhiteSpace(saveSymptomDTO.SymptomName);

        entity.Id = saveSymptomDTO.SymptomId;
        entity.Name = saveSymptomDTO.SymptomName;
        entity.IsActive = true;
        entity.UpdatedOn = DateTime.UtcNow;

        return entity;
    }
}