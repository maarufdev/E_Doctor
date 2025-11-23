using E_Doctor.Application.DTOs.Common.UserAccountDTOs;
using E_Doctor.Patient.ViewModels.PatientVM;

namespace E_Doctor.Patient.Helpers
{
    public static class UserDetailsMapper
    {
        public static UpsertProfileVM ToUpsertViewModal(this UserDetailsResponseDTO dto)
        {
            ArgumentNullException.ThrowIfNull(nameof(dto));

            var vm = new UpsertProfileVM
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                MiddleName = dto.MiddleName,
                DOB = dto.DateOfBirth,
                Religion = dto.Religion,
                Gender = dto.Gender,
                Address = dto.Address,
                CivilStatus = dto.CivilStatus,
                Occupation = dto.Occupation,
                Nationality = dto.Nationality,
            };


            if (dto.PatientPastMedicalRecord != null) 
            {
                var pastRecord = dto.PatientPastMedicalRecord;
                vm.PastMedicalRecord = new PastMedicalRecordsVM
                {
                    PreviousHospitalization = pastRecord.PreviousHospitalization,
                    PastSurgery = pastRecord.PastSurgery,
                    Diabetes = pastRecord.Diabetes,
                    Hypertension = pastRecord.Hypertension,
                    AllergyToMeds = pastRecord.AllergyToMeds,
                    HeartProblem = pastRecord.HeartProblem,
                    Asthma = pastRecord.Asthma,
                    FoodAllergies = pastRecord.FoodAllergies,
                    Cancer = pastRecord.Cancer,
                    OtherIllnesses = pastRecord.OtherIllnesses,
                    MaintenanceMeds = pastRecord.MaintenanceMeds,
                    OBGyneHistory = pastRecord.OBGyneHistory,
                };
            }

            if(dto.PatientFamilyHistory != null)
            {
                var famHistory = dto.PatientFamilyHistory;

                vm.FamilyHistory = new FamilyHistoryVM
                {
                    PTB = famHistory.PTB,
                    Hypertension = famHistory.Hypertension,
                    None = famHistory.None,
                    Diabetes = famHistory.Diabetes,
                    Asthma = famHistory.Asthma,
                    Cancer = famHistory.Cancer,
                    Others = famHistory.Others,
                };
            }

            if(dto.PatientPersonalHistory != null)
            {
                var personalHistory = dto.PatientPersonalHistory;

                vm.PersonalHistory = new PersonalHistoryVM
                {
                    Smoker = personalHistory.Smoker,
                    AlchoholBeverageDrinker = personalHistory.AlchoholBeverageDrinker,
                    IllicitDrugUser = personalHistory.IllicitDrugUser,
                    Others = personalHistory.Others,
                };
            }

            return vm;
        }

        public static string GetMissingFields(this UpsertProfileVM vm)
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

            if (vm.DOB == null || vm.DOB == DateTime.MinValue) emptyRequiredFields.Add(nameof(vm.DOB));

            if (emptyRequiredFields.Any())
            {
                result = string.Join(", ", emptyRequiredFields.Select(f => f));
            }

            return result;
        }

        public static SaveUserDetailsRequest ToDto(this UpsertProfileVM vm)
        {
            var dto = new SaveUserDetailsRequest();

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
            };

            if (vm.PastMedicalRecord is not null)
            {
                var medRecord = vm.PastMedicalRecord;
                PatientPastMedicalRecordDTO patientPastMedRecord = new()
                {
                    PreviousHospitalization = medRecord.PreviousHospitalization,
                    PastSurgery = medRecord.PastSurgery,
                    Diabetes = medRecord.Diabetes,
                    Hypertension = medRecord.Hypertension,
                    AllergyToMeds = medRecord.AllergyToMeds,
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

            if (vm.FamilyHistory is not null)
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

            if (vm.PersonalHistory is not null)
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
    }
}
