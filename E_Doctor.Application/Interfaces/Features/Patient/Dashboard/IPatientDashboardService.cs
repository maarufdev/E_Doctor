using E_Doctor.Application.DTOs.Common.CustomResultDTOs;
using E_Doctor.Application.DTOs.Dashboard;

namespace E_Doctor.Application.Interfaces.Features.Patient.Dashboard;
public interface IPatientDashboardService
{
    Task<Result<GetDashboardDetailsDTO>> GetDashboardDetails();
}
