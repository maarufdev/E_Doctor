namespace E_Doctor.Patient.ViewModels.RegisterPatientVM
{
    public class PersonalHistoryVM
    {
        public bool Smoker { get; set; }
        public bool AlchoholBeverageDrinker { get; set; }
        public bool IllicitDrugUser { get; set; }
        public string? Others { get; set; } = string.Empty;
    }
}