namespace E_Doctor.Application.DTOs.Common.ExportIllnessDTOs;
public class ExportIllnessConfigDTO
{
    public IEnumerable<ExportSymptomDTO> Symptoms { get; set; } = [];
    public IEnumerable<ExportIllnessDTO> Illnesses { get; set; } = [];
    public IEnumerable<ExportRulesDTO> Rules { get; set; } = [];
}