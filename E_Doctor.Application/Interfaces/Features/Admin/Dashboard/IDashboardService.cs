using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Dashboard;

namespace E_Doctor.Application.Interfaces.Features.Admin.Dashboard;
public interface IDashboardService
{
    Task<Result<GetDashboardDetailsDTO>> GetDashboardDetails();
}
