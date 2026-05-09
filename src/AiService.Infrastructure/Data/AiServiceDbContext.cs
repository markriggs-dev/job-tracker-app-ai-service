using Microsoft.EntityFrameworkCore;
using AiService.Core.Models;

namespace AiService.Infrastructure.Data;

public class AiServiceDbContext : DbContext
{
    public AiServiceDbContext(DbContextOptions<AiServiceDbContext> options) : base(options) { }

    public DbSet<AiProfile> AiProfiles => Set<AiProfile>();
    public DbSet<GeneratedResume> GeneratedResumes => Set<GeneratedResume>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AiProfile>(e =>
        {
            e.ToTable("ai_profiles");
            e.HasKey(x => x.Id);
            e.Property(x => x.UserId).HasMaxLength(256).IsRequired();
            e.Property(x => x.Name).HasMaxLength(256).IsRequired();
            e.Property(x => x.Instructions).IsRequired();
            e.HasIndex(x => x.UserId);
        });

        modelBuilder.Entity<GeneratedResume>(e =>
        {
            e.ToTable("generated_resumes");
            e.HasKey(x => x.Id);
            e.Property(x => x.UserId).HasMaxLength(256).IsRequired();
            e.Property(x => x.FileName).HasMaxLength(512).IsRequired();
            e.Property(x => x.StorageKey).HasMaxLength(1024).IsRequired();
            e.HasIndex(x => new { x.JobRequisitionId, x.UserId });
        });
    }
}
