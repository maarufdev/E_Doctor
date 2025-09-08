namespace E_Doctor.Application.DTOs.Dashboard;
public sealed record GetDashboardDetailsDTO(
    GetDashboardCardDetailsDTO GetDashboardCardDetails
    )
{
    public static GetDashboardDetailsDTO Create(GetDashboardCardDetailsDTO getDashboardCardDetailsDTO)
    {
        return new GetDashboardDetailsDTO(getDashboardCardDetailsDTO);
    }
}