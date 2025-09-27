using E_Doctor.Core.Domain.Entities.Admin;
using E_Doctor.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Infrastructure.Data;
public class AdminAppDbContext : IdentityDbContext<AppUserIdentity, IdentityRole<int>, int>
{
    public AdminAppDbContext(DbContextOptions<AdminAppDbContext> options): base(options)
    {
        
    }

    public DbSet<SymptomEntity> Symptoms { get; set; }
    public DbSet<IllnessEntity> Illnesses { get; set; }
    public DbSet<IllnessRuleEntity> IllnessRules { get; set; }
    public DbSet<DiagnosisTestEntity> DiagnosisTest { get; set; }
    public DbSet<AppUserIdentity> UserAccounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        var currentDate = DateTime.UtcNow;
        #region Symptoms Seeding and Config
        modelBuilder.Entity<SymptomEntity>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            entity.HasData(new SymptomEntity { Id = 1, Name = "Fever", IsActive = true, CreatedOn = new DateTime(2025, 8, 9), });
        });
        #endregion

        #region Illness Entity Seeding and Config
        modelBuilder.Entity<IllnessEntity>(entity =>
        {
            entity.HasKey(s => s.Id);

            // Seeding

            entity.HasData(
                new IllnessEntity
                {
                    Id = 1,
                    IllnessName = "Flu",
                    Description = "Common",
                    CreatedOn = new DateTime(2025, 8, 9),
                    IsActive = true,
               });
        });
        #endregion

        #region DiseaseRules
        modelBuilder.Entity<IllnessRuleEntity>(entity =>
        {
            entity.HasKey(dr => new { dr.IllnessId, dr.SymptomId });

            entity.HasOne(dr => dr.Illness)
            .WithMany(d => d.Rules)
            .HasForeignKey(dr => dr.IllnessId);

            entity.HasOne(dr => dr.Symptom)
            .WithMany(d => d.Rules)
            .HasForeignKey(dr => dr.SymptomId);

            entity.HasData(
                new IllnessRuleEntity
                {
                    IllnessId = 1,
                    SymptomId = 1,
                    IsActive = true,
                });
        });
        #endregion
    }
}