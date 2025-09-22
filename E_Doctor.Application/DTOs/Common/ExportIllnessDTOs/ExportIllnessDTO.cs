namespace E_Doctor.Application.DTOs.Common.ExportIllnessDTOs;
public sealed record ExportIllnessDTO(
    int IllnessId,
    string IllnessName,
    string Description,
    string Prescription,
    string Notes
);
