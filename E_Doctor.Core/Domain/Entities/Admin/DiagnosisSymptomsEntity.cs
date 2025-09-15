namespace E_Doctor.Core.Domain.Entities.Admin;
public class DiagnosisSymptomsEntity
{
    public int Id { get; set; }
    public int DiagnosisId { get; set; }
    public virtual DiagnosisEntity? Diagnosis { get; set; }
    public int IllnessId { get; set; }
    public virtual IllnessEntity? Illness { get; set; }
    public decimal Score { get; set; }
}