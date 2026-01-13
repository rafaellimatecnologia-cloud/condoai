using CondoAI.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace CondoAI.Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ShiftTemplate> ShiftTemplates => Set<ShiftTemplate>();
    public DbSet<SystemConfig> SystemConfigs => Set<SystemConfig>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SyncBatch> SyncBatches => Set<SyncBatch>();
    public DbSet<PresenceRecord> PresenceRecords => Set<PresenceRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SystemConfig>()
            .HasIndex(c => c.Key)
            .IsUnique();

        modelBuilder.Entity<SyncBatch>()
            .HasIndex(b => b.ClientEventId)
            .IsUnique();
    }
}
