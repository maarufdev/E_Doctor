using E_Doctor.Core.Domain.Entities.Admin;
using E_Doctor.Core.Domain.Entities.Common;
using E_Doctor.Core.Domain.Entities.Patient;
using E_Doctor.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Infrastructure.Data;
public class AppDbContext : IdentityDbContext<AppUserIdentity, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }

    public DbSet<SymptomEntity> Symptoms { get; set; }
    public DbSet<IllnessEntity> Illnesses { get; set; }
    public DbSet<IllnessRuleEntity> IllnessRules { get; set; }
    public DbSet<DiagnosisEntity> Diagnosis { get; set; }
    public DbSet<DiagnosisIllnessesEntity> DiagnosisIllnesses { get; set; }
    public DbSet<DiagnosisSymptomsEntity> DiagnosisSymptoms { get; set; }
    public DbSet<AppUserIdentity> UserAccounts { get; set; }
    public DbSet<UserActivityEntity> UserActivities { get; set; }
    public DbSet<PatientInformationEntity> PatientInformations { get; set; }
    public DbSet<PatientPastMedicalRecordEntity> PatientPastMedicalRecords { get; set; }
    public DbSet<PatientFamilyHistoryEntity> PatientFamilyHistory { get; set; }
    public DbSet<PatientPersonalHistoryEntity> PatientPersonalHistory { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);
        
        var currentDate = DateTime.UtcNow;
        #region Symptoms Seeding and Config
        modelBuilder.Entity<SymptomEntity>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id)
                .ValueGeneratedOnAdd();
        });
        #endregion

        #region Illness Entity Seeding and Config
        modelBuilder.Entity<IllnessEntity>(entity =>
        {
            entity.HasKey(s => s.Id);
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
        });
        #endregion

        modelBuilder.Entity<DiagnosisEntity>(e =>
        {
            e.HasKey(pd => pd.Id);
            e.HasOne<AppUserIdentity>()
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DiagnosisIllnessesEntity>(e =>
        {
            e.HasKey(pd => pd.Id);
            e.Property(e => e.Score)
                .HasPrecision(18, 4);
            
            e.HasOne(d => d.Diagnosis)
            .WithMany(d => d.DiagnosIllnesses)
            .HasForeignKey(d => d.DiagnosisId);
        });

        modelBuilder.Entity<DiagnosisSymptomsEntity>(e =>
        {
            e.HasKey(pd => pd.Id);

            e.HasOne(d => d.Diagnosis)
            .WithMany(d => d.DiagnosSymptoms)
            .HasForeignKey(d => d.DiagnosisId);
        });

        modelBuilder.Entity<UserActivityEntity>(e =>
        {
            e.HasKey(pd => pd.Id);
        });

        modelBuilder.Entity<PatientInformationEntity>(e =>
        {
            e.HasKey(pd => pd.Id);

            e.HasOne<AppUserIdentity>()
            .WithOne(au => au.PatientInformation)
            .HasForeignKey<PatientInformationEntity>(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);


            // PatientInformation has one PatientPastMedicalRecord
            e.HasOne(p => p.PatientPastMedicalRecord)
            .WithOne(pr => pr.PatientInformation)
            .HasForeignKey<PatientPastMedicalRecordEntity>(ppmr => ppmr.PatientInfoId)
            .OnDelete(DeleteBehavior.Cascade);


            // PatientInformation has one PatientFamilyHistoryEntity
            e.HasOne(p => p.PatientFamilyHistory)
            .WithOne(pr => pr.PatientInformation)
            .HasForeignKey<PatientFamilyHistoryEntity>(ppmr => ppmr.PatientInfoId)
            .OnDelete(DeleteBehavior.Cascade);


            // PatientInformation has one PatientPersonalHistoryEntity
            e.HasOne(p => p.PatientPersonalHistory)
            .WithOne(pr => pr.PatientInformation)
            .HasForeignKey<PatientPersonalHistoryEntity>(ppmr => ppmr.PatientInfoId)
            .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PatientPastMedicalRecordEntity>(e =>
        {
            e.HasKey(pd => pd.Id);
        });

        modelBuilder.Entity<PatientFamilyHistoryEntity>(e =>
        {
            e.HasKey(pd => pd.Id);
        });

        modelBuilder.Entity<PatientPersonalHistoryEntity>(e =>
        {
            e.HasKey(pd => pd.Id);
        });

        // Infrastructure Layer: Inside YourDbContext.OnModelCreating
        modelBuilder.Entity<AppUserIdentity>(e =>
        {
            // AppUserIdentity has one PatientInformation
            e.HasOne(au => au.PatientInformation)

             // PatientInformation has one AppUserIdentity (no navigation property here!)
             // We use WithOne() without a parameter.
             .WithOne()

             // The foreign key is located on the PatientInformationEntity
             .HasForeignKey<PatientInformationEntity>(pi => pi.UserId)

             .OnDelete(DeleteBehavior.Cascade);
        });

        base.OnModelCreating(modelBuilder);
    }
}