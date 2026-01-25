namespace E_Doctor.Application.DTOs.Diagnosis;

public sealed record PrintReceiptInfoDTO(
    string Illness,
    string Fullname,
    int Age,
    string Gender,
    string Disclaimer
    );
