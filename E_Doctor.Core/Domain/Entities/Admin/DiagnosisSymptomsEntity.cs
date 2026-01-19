namespace E_Doctor.Core.Domain.Entities.Admin;
public class DiagnosisSymptomsEntity : BaseEntity
{
    public int DiagnosisId { get; set; }
    public DiagnosisEntity? Diagnosis { get; set; }
    public int SymptomId { get; set; }
    public string? SymptomName { get; set; }
}