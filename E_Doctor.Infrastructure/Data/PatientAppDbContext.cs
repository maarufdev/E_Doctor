using E_Doctor.Core.Domain.Entities.Patient;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Infrastructure.Data
{
    public class PatientAppDbContext : DbContext
    {
        public PatientAppDbContext(DbContextOptions<PatientAppDbContext> options) : base(options)
        {
        }

        public DbSet<PatientSymptomEntity> PatientSymptoms { get; set; }
        public DbSet<PatientIllnessEntity> PatientIllnesses { get; set; }
        public DbSet<PatientRulesEntity> PatientIllnesRules { get; set; }
        public DbSet<PatientDiagnosisEntity> PatientDiagnosis { get; set; }

        public DbSet<PatientDiagnosisIllnessEntity> PatientDiagnosisIllnesses { get; set; }
        public DbSet<PatientDiagnosisSymptomEntity> PatientDiagnosisSymptoms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PatientSymptomEntity>(e =>
            {
                e.HasKey(pk => pk.SymptomId);
            });

            modelBuilder.Entity<PatientIllnessEntity>(e =>
            {
                e.HasKey(pk => pk.IllnessId);
            });

            modelBuilder.Entity<PatientRulesEntity>(e =>
            {
                e.HasKey(dr => new { dr.IllnessId, dr.SymptomId });
                
                e.HasOne(r => r.Illness)
                .WithMany(i => i.Rules)
                .HasForeignKey(r => r.IllnessId);

                e.HasOne(r => r.Symptom)
                .WithMany(s => s.Rules)
                .HasForeignKey(r => r.SymptomId);

            });

            modelBuilder.Entity<PatientDiagnosisEntity>(e =>
            {
                e.HasKey(pd => pd.Id);
            });

            modelBuilder.Entity<PatientDiagnosisIllnessEntity>(e =>
            {
                e.HasKey(pd => pd.Id);

                e.HasOne(d => d.Diagnosis)
                .WithMany(d => d.DiagnosIllnesses)
                .HasForeignKey(d => d.DiagnosisId);
            });

            modelBuilder.Entity<PatientDiagnosisSymptomEntity>(e =>
            {
                e.HasKey(pd => pd.Id);

                e.HasOne(d => d.Diagnosis)
                .WithMany(d => d.DiagnosSymptoms)
                .HasForeignKey(d => d.DiagnosisId);
            });
        }
    }
}