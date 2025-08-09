using E_Doctor.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace E_Doctor.Infrastructure.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
    {
        
    }
    public DbSet<SymptomEntity> Symptoms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        var currentDate = DateTime.UtcNow;
        modelBuilder.Entity<SymptomEntity>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            entity.HasData(new SymptomEntity { Id = 1, Name = "Fever", IsActive = true, CreatedOn = new DateTime(2025, 8, 9), });
        });
    }
}
