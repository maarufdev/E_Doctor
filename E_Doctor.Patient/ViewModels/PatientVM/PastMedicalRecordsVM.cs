namespace E_Doctor.Patient.ViewModels.PatientVM
{
    public class PastMedicalRecordsVM
    {
        public bool PreviousHospitalization { get; set; }
        public string PreviousHospitalizationText { get; set; } = string.Empty;
        public bool PastSurgery { get; set; }
        public bool Diabetes { get; set; }
        public bool Hypertension { get; set; }
        public bool AllergyToMeds { get; set; }
        public string MedAllergyText { get; set; } = string.Empty;
        public bool HeartProblem { get; set; }
        public bool Asthma { get; set; }
        public bool FoodAllergies { get; set; }
        public bool Cancer { get; set; }
        public string? OtherIllnesses { get; set; } = string.Empty;
        public string? MaintenanceMeds { get; set; } = string.Empty;
        public string? OBGyneHistory { get; set; } = string.Empty;
    }
}
