using E_Doctor.Core.Domain.Entities.Admin;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Application.Interfaces.Data
{
    public interface IAppDbContext
    {
        DbSet<SymptomEntity> Symptoms { get; }
        DbSet<IllnessEntity> Illnesses { get; }
        DbSet<IllnessRuleEntity> IllnessRules { get; }
        DbSet<DiagnosisEntity> DiagnosisTest { get; }
        //DbSet<AppUserIdentity> UserAccounts { get; }
        DbSet<UserEntity> UserAccounts { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
