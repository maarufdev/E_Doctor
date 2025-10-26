namespace E_Doctor.Application.DTOs.Dashboard;
public sealed record GetDashboardRecentDiagnosisDTO(string Date, string Symptoms, string Result, string DisplayName = default);