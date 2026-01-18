using E_Doctor.Application.DTOs.Common.UserAccountDTOs;
using E_Doctor.Patient.ViewModels.PatientVM;

namespace E_Doctor.Patient.Helpers
{
    public static class RegisterUserMapper
    {
        public static RegisterPatientDTO ToDto(this RegisterUserVM vm)
        {
            var dto = new RegisterPatientDTO();

            dto = dto with
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                MiddleName = vm.MiddleName,
                DateOfBirth = vm.DOB,
                Religion = vm.Religion,
                Gender = vm.Gender,
                Address = vm.Address,
                CivilStatus = vm.CivilStatus,
                Occupation = vm.Occupation,
                Nationality = vm.Nationality,
                UserName = vm.Username,
                Password = vm.Password
            };

            if(vm.PastMedicalRecord is not null)
            {
                var medRecord = vm.PastMedicalRecord;
                PatientPastMedicalRecordDTO patientPastMedRecord = new()
                {
                    PreviousHospitalization = medRecord.PreviousHospitalization,
                    PreviousHospitalizationText = medRecord.PreviousHospitalization ? medRecord.PreviousHospitalizationText : string.Empty,
                    PastSurgery = medRecord.PastSurgery,
                    Diabetes = medRecord.Diabetes,
                    Hypertension = medRecord.Hypertension,
                    AllergyToMeds = medRecord.AllergyToMeds,
                    MedAllergyText = medRecord.AllergyToMeds ? medRecord.MedAllergyText : string.Empty,
                    HeartProblem = medRecord.HeartProblem,
                    Asthma = medRecord.Asthma,
                    FoodAllergies = medRecord.FoodAllergies,
                    Cancer = medRecord.Cancer,
                    OtherIllnesses = medRecord.OtherIllnesses,
                    MaintenanceMeds = medRecord.MaintenanceMeds,
                    OBGyneHistory = medRecord.OBGyneHistory,
                };

                dto = dto with
                {
                    PatientPastMedicalRecord = patientPastMedRecord,
                };
            }

            if(vm.FamilyHistory is not null)
            {
                var famHistory = vm.FamilyHistory;
                PatientFamilyHistoryDTO patientFamilyHistoryDTO = new()
                {
                    PTB = famHistory.PTB,
                    Hypertension = famHistory.Hypertension,
                    Cancer = famHistory.Cancer,
                    Cardiac = famHistory.Cardiac,
                    None = famHistory.None,
                    Diabetes = famHistory.Diabetes,
                    Asthma = famHistory.Asthma,
                    Others = famHistory.Others,
                };

                dto = dto with
                {
                    PatientFamilyHistory = patientFamilyHistoryDTO,
                };
            }

            if(vm.PersonalHistory is not null)
            {
                var pHistory = vm.PersonalHistory;
                PatientPersonalHistoryDTO pHistoryDTO = new()
                {
                    Smoker = pHistory.Smoker,
                    AlchoholBeverageDrinker = pHistory.AlchoholBeverageDrinker,
                    IllicitDrugUser = pHistory.IllicitDrugUser,
                    Others = pHistory.Others,
                };

                dto = dto with
                {
                    PatientPersonalHistory = pHistoryDTO,
                };
            }

            return dto;
        }

        public static string GetMissingFields(this RegisterUserVM vm)
        {
            var result = string.Empty;
            var emptyRequiredFields = new List<string>();

            if (string.IsNullOrWhiteSpace(vm.LastName)) emptyRequiredFields.Add(nameof(vm.LastName));
            if (string.IsNullOrWhiteSpace(vm.FirstName)) emptyRequiredFields.Add(nameof(vm.FirstName));
            if (string.IsNullOrWhiteSpace(vm.MiddleName)) emptyRequiredFields.Add(nameof(vm.MiddleName));
            if (string.IsNullOrWhiteSpace(vm.Gender)) emptyRequiredFields.Add(nameof(vm.Gender));
            if (string.IsNullOrWhiteSpace(vm.Religion)) emptyRequiredFields.Add(nameof(vm.Religion));
            if (string.IsNullOrWhiteSpace(vm.CivilStatus)) emptyRequiredFields.Add(nameof(vm.CivilStatus));
            if (string.IsNullOrWhiteSpace(vm.Nationality)) emptyRequiredFields.Add(nameof(vm.Nationality));
            if (string.IsNullOrWhiteSpace(vm.Password)) emptyRequiredFields.Add(nameof(vm.Password));
            if (string.IsNullOrWhiteSpace(vm.ConfirmPassword)) emptyRequiredFields.Add(nameof(vm.ConfirmPassword));
            if (string.IsNullOrWhiteSpace(vm.Username)) emptyRequiredFields.Add(nameof(vm.Username));

            if(vm.DOB == null || vm.DOB == DateTime.MinValue) emptyRequiredFields.Add(nameof(vm.DOB));

            var pastMecicalRecs = vm.PastMedicalRecord;

            if (pastMecicalRecs.PreviousHospitalization && string.IsNullOrWhiteSpace(pastMecicalRecs.PreviousHospitalizationText))
            {
                emptyRequiredFields.Add(nameof(pastMecicalRecs.PreviousHospitalizationText));
            }

            if (pastMecicalRecs.AllergyToMeds && string.IsNullOrWhiteSpace(pastMecicalRecs.MedAllergyText))
            {
                emptyRequiredFields.Add(nameof(pastMecicalRecs.MedAllergyText));
            }

            if (emptyRequiredFields.Any())
            {
                result = string.Join(", ", emptyRequiredFields.Select(f => f));
            }

            return result;
        }
    }
}