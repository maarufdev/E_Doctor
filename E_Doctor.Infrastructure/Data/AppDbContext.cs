using E_Doctor.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Infrastructure.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }
    public DbSet<SymptomEntity> Symptoms { get; set; }
    public DbSet<DiseaseEntity> Diseases { get; set; }
    public DbSet<DiseaseRuleEntity> DiseaseRules { get; set; }

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

        #region Disease Entity Seeding and Config
        modelBuilder.Entity<DiseaseEntity>(entity =>
        {
            entity.HasKey(s => s.Id);

            // Seeding

            entity.HasData(
                new DiseaseEntity
                {
                    Id = 1,
                    DiseaseName = "Flu",
                    Description = "Common",
                    CreatedOn = new DateTime(2025, 8, 9),
                    IsActive = true,
               });
        });
        #endregion

        #region DiseaseRules
        modelBuilder.Entity<DiseaseRuleEntity>(entity =>
        {
            entity.HasKey(dr => new { dr.DiseaseId, dr.SymptomId });

            entity.HasOne(dr => dr.Disease)
            .WithMany(d => d.Rules)
            .HasForeignKey(dr => dr.DiseaseId);

            entity.HasOne(dr => dr.Symptom)
            .WithMany(d => d.Rules)
            .HasForeignKey(dr => dr.SymptomId);

            entity.HasData(
                new DiseaseRuleEntity
                {
                    DiseaseId = 1,
                    SymptomId = 1,
                    Condition = Core.Constants.Enums.DiseaseRuleConditionEnum.IsLessThan,
                    Days = 2,
                    IsActive = true,
                });
        });
        #endregion
    }
}