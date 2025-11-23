using E_Doctor.Core.Constants.Enums;
using E_Doctor.Core.Domain.Entities.Admin;
using E_Doctor.Core.Domain.Entities.Common;
using E_Doctor.Core.Domain.Entities.Patient;
using E_Doctor.Core.Domain.Entities.Patient.PhysicalExam;
using E_Doctor.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Infrastructure.Data;
public class AppDbContext : IdentityDbContext<AppUserIdentity, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
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
    public DbSet<PhysicalExamEntity> PhysicalExams { get; set; }
    public DbSet<PhysicalExamFindingsEntity> PhysicalExamFindings { get; set; }
    public DbSet<PhysicalExamItemsEntity> PhysicalExamItems { get; set; }

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

        #region Physical Exam Sections
        modelBuilder.Entity<PhysicalExamEntity>(e =>
        {
            e.HasKey(e => e.Id);

            e.HasMany(e => e.PhysicalExamFindings)
            .WithOne(e => e.PhysicalExam)
            .HasForeignKey(e => e.PhysicalExamId)
            .OnDelete(DeleteBehavior.ClientCascade);
        });
        #endregion

        #region Physical Exam Findings
        modelBuilder.Entity<PhysicalExamFindingsEntity>(e =>
        {
            e.HasKey(p => p.Id);

            e.HasOne(p => p.PhysicalExamItem)
            .WithMany()
            .HasForeignKey(p => p.PhysicalItemId)
            .OnDelete(DeleteBehavior.Restrict);
        });
        #endregion

        #region Physical exam items
        modelBuilder.Entity<PhysicalExamItemsEntity>(e =>
        {
            e.HasKey(e => e.Id);
        });
        #endregion

        var seedDate = DateTime.UtcNow; // CURRENT UTC DATE (at migration time)

        #region Physical Exam items seeding
        modelBuilder.Entity<PhysicalExamItemsEntity>().HasData(
     new PhysicalExamItemsEntity
     {
         Id = 1,
         Label = "General",
         SortOrder = 1,
         CreatedOn = seedDate,
         IsActive = false,
     },
     new PhysicalExamItemsEntity
     {
         Id = 2,
         Label = "Head",
         SortOrder = 2,
         CreatedOn = seedDate,
         IsActive = true,
     },
     new PhysicalExamItemsEntity
     {
         Id = 3,
         Label = "Eyes",
         SortOrder = 3,
         CreatedOn = seedDate,
         IsActive = true
     },
     new PhysicalExamItemsEntity
     {
         Id = 4,
         Label = "Ears",
         SortOrder = 4,
         CreatedOn = seedDate,
         IsActive = true
     },
     new PhysicalExamItemsEntity
     {
         Id = 5,
         Label = "Nose",
         SortOrder = 5,
         CreatedOn = seedDate,
         IsActive = true
     },
     new PhysicalExamItemsEntity
     {
         Id = 6,
         Label = "Throat",
         SortOrder = 6,
         CreatedOn = seedDate,
         IsActive = true
     },
     new PhysicalExamItemsEntity
     {
         Id = 7,
         Label = "Neck",
         SortOrder = 7,
         CreatedOn = seedDate,
         IsActive = true
     },
     new PhysicalExamItemsEntity
     {
         Id = 8,
         Label = "Breast",
         SortOrder = 8,
         CreatedOn = seedDate,
         IsActive = true
     },
     new PhysicalExamItemsEntity
     {
         Id = 9,
         Label = "Chest/Lungs",
         SortOrder = 9,
         CreatedOn = seedDate,
         IsActive = true
     },
     new PhysicalExamItemsEntity
     {
         Id = 10,
         Label = "Heart",
         SortOrder = 10,
         CreatedOn = seedDate,
         IsActive = true
     },
      new PhysicalExamItemsEntity
      {
          Id = 11,
          Label = "Abdomen",
          SortOrder = 11,
          CreatedOn = seedDate,
          IsActive = true
      },
       new PhysicalExamItemsEntity
       {
           Id = 12,
           Label = "Gut",
           SortOrder = 12,
           CreatedOn = seedDate,
           IsActive = true
       },
        new PhysicalExamItemsEntity
        {
            Id = 13,
            Label = "Back",
            SortOrder = 13,
            CreatedOn = seedDate,
            IsActive = true
        },
         new PhysicalExamItemsEntity
         {
             Id = 14,
             Label = "Extremities",
             SortOrder = 14,
             CreatedOn = seedDate,
             IsActive = true
         },
          new PhysicalExamItemsEntity
          {
              Id = 15,
              Label = "Neurologic",
              SortOrder = 15,
              CreatedOn = seedDate,
              IsActive = true
          },
           new PhysicalExamItemsEntity
           {
               Id = 16,
               Label = "Skin",
               SortOrder = 16,
               CreatedOn = seedDate,
               IsActive = true
           },
            new PhysicalExamItemsEntity
            {
                Id = 17,
                Label = "Others",
                SortOrder = 17,
                CreatedOn = seedDate,
                IsActive = true
            }
 );



        #endregion
    }
}