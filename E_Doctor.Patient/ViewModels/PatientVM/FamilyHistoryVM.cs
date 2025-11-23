using System.ComponentModel.DataAnnotations;

namespace E_Doctor.Patient.ViewModels.PatientVM
{
    public class FamilyHistoryVM
    {
        public bool PTB { get; set; }
        public bool Hypertension { get; set; }
        public bool Cardiac { get; set; }
        public bool None { get; set; } = true;
        public bool Diabetes { get; set; }
        public bool Asthma { get; set; }
        public bool Cancer { get; set; }
        public string? Others { get; set; } = string.Empty;
    }
}