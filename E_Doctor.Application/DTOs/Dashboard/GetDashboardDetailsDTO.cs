namespace E_Doctor.Application.DTOs.Dashboard;
public sealed record GetDashboardDetailsDTO(
    GetDashboardCardDetailsDTO GetDashboardCardDetails,
    List<GetDashboardRecentDiagnosisDTO> RecentDiagnosis
    )
{
    public static GetDashboardDetailsDTO Create(GetDashboardCardDetailsDTO getDashboardCardDetailsDTO, List<GetDashboardRecentDiagnosisDTO> recentDiagnosis)
    {
        return new GetDashboardDetailsDTO(getDashboardCardDetailsDTO, recentDiagnosis);
    }
}