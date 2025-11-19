namespace E_Doctor.Application.DTOs.Diagnosis.PhysicalExams;
public sealed record PhysicalExamDTO
{
    public int ExamId { get; set; }
    public string BP { get; set; } = string.Empty;
    public string HR { get; set; } = string.Empty;
    public string RR { get; set; } = string.Empty;
    public string Temp { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
    public string O2Sat { get; set; } = string.Empty;

    public List<PhysicalExamFindingDTO> PhysicalExamFindings { get; set; } = new();
}