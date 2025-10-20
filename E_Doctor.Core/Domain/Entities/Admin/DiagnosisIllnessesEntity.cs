using E_Doctor.Core.Domain.Entities.Patient;

namespace E_Doctor.Core.Domain.Entities.Admin;
public class DiagnosisIllnessesEntity : BaseEntity
{
    public int DiagnosisId { get; set; }
    public DiagnosisEntity? Diagnosis { get; set; }
    public string Illness { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public decimal Score { get; set; }
}