namespace E_Doctor.Application.DTOs.Dashboard;
public sealed record GetDashboardCardDetailsDTO(
    int TotalPendingPhysicalExams,
    int TotalCompletedPhysicalExams,
    int TotalPatients
    )
{
    public static GetDashboardCardDetailsDTO Create(
        int totalPendingPhysicalExam, 
        int totalCompletedPhysicalExam,
        int totalPatient)
    {
        return new GetDashboardCardDetailsDTO(totalPendingPhysicalExam, totalCompletedPhysicalExam, totalPatient);
    }
}