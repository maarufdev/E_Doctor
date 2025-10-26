using E_Doctor.Core.Domain.Entities.Admin;

namespace E_Doctor.Infrastructure.Identity;
public class DiagnosisWithUser : DiagnosisEntity
{
    public AppUserIdentity? User { get; set; }
}