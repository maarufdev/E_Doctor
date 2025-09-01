namespace E_Doctor.Core.Domain.Entities.Patient;
public class PatientSymptomEntity
{
    public int SymptomId { get; set; }
    public string SymptomName { get; set; } = string.Empty;
    public DateTime UpdatedOn { get; set; }
}