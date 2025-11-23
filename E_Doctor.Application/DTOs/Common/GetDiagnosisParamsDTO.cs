namespace E_Doctor.Application.DTOs.Common;
public sealed record GetDiagnosisParamsDTO(string SearchText, int PageNumber, int PageSize);