namespace E_Doctor.Application.DTOs.Dashboard;
public sealed record GetDashboardCardDetailsDTO(
    int TotalSymptoms,
    int TotalIllnesses,
    int TotalRules,
    int TotalPatients
    )
{
    public static GetDashboardCardDetailsDTO Create(
        int symptomsCount, 
        int illnessesCount, 
        int rulesCount,
        int totalPatient)
    {
        return new GetDashboardCardDetailsDTO(symptomsCount, illnessesCount, rulesCount, totalPatient);
    }
}