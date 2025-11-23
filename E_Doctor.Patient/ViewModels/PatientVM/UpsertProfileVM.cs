using System.ComponentModel.DataAnnotations;

namespace E_Doctor.Patient.ViewModels.PatientVM
{
    public class UpsertProfileVM
    {

        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public DateTime? DOB { get; set; }
        public string? Religion { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? CivilStatus { get; set; }
        public string? Occupation { get; set; }
        public string? Nationality { get; set; }
        public PastMedicalRecordsVM PastMedicalRecord { get; set; } = new();
        public FamilyHistoryVM FamilyHistory { get; set; } = new FamilyHistoryVM();
        public PersonalHistoryVM PersonalHistory { get; set; } = new PersonalHistoryVM();
    }
}
