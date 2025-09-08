namespace E_Doctor.Application.DTOs.Dashboard;
public sealed record GetDashboardCardDetailsDTO(
    int TotalSymptoms,
    int TotalIllnesses,
    int TotalRules
    )
{
    public static GetDashboardCardDetailsDTO Create(
        int symptomsCount, 
        int illnessesCount, 
        int rulesCount)
    {
        return new GetDashboardCardDetailsDTO(symptomsCount, illnessesCount, rulesCount);
    }
}