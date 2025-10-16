namespace E_Doctor.Application.DTOs.Common.ExportIllnessDTOs;

public sealed record ExportSymptomDTO(
    int SymptomId, 
    string SymptomName,
    string QuestionText
);
