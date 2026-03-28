using APBD1.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace APBD1.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Device> Devices => Set<Device>();
    public DbSet<Lease> Leases => Set<Lease>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(user =>
        {
            user.UseTptMappingStrategy();
            user.HasKey(x => x.Id);
        });
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.IndexNumber)
            .IsUnique();
        modelBuilder.Entity<Employee>();

        modelBuilder.Entity<Device>(d =>
        {
            d.UseTptMappingStrategy();
            d.HasKey(x => x.Id);
            d.HasIndex(x => x.Name).IsUnique();
        });
        modelBuilder.Entity<Camera>();
        modelBuilder.Entity<Laptop>().OwnsOne(l => l.ScreenRatio);
        modelBuilder.Entity<Projector>().OwnsOne(p => p.MaxScreenRatio);

        modelBuilder.Entity<Lease>(lease =>
        {
            lease.HasKey(e => e.Id);
            lease.HasOne(l => l.Leaser)
                .WithMany(l => l.Leases)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            lease.HasOne(l => l.Device)
                .WithMany(d => d.Leases)
                .HasForeignKey(l => l.DeviceId)
                .OnDelete(DeleteBehavior.Restrict);
            lease.HasIndex(l => l.UserId);
            lease.HasIndex(l => l.DeviceId);
        });
    }
}