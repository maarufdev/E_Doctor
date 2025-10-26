namespace E_Doctor.Core.Constants.Enums;
public enum UserActivityTypes
{
    // General
    Login,
    Register,
    Logout,
    ResetPassword,

    // Illness
    CreateIllness,
    UpdateIllness,
    RemoveIllness,

    // Symptoms
    CreateSymptom,
    UpdateSymptom,
    RemoveSymptom,
    
    // Admin
    AdminCreateUser,
    AdminUpdateUser,
    AdminResetUserPassword,
    AdminReactivateUser,
    AdminArchiveUser,
    AdminDeletedUser,
    AdminDeleteDiagnosis,

    // Patient
    PatientDiagnosis,
    PatientDeleteDiagnosis,

    // System Task
    SystemArchiveUser
}