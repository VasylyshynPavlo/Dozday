using Dozday.Core.Models;
using Dozday.Core.Models.Archive;
using Microsoft.EntityFrameworkCore;

namespace Dozday.Data;

public class DozdayDbContext : DbContext
{
    public DozdayDbContext(DbContextOptions<DozdayDbContext> options)
        : base(options)
    {
    }

    public override int SaveChanges()
    {
        NormalizeDateTimes();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        NormalizeDateTimes();
        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }
    public DbSet<QualityAndAssurance> QualityAndAssurances { get; set; }

    //archive
    public DbSet<EventArchived> EventArchive { get; set; }
    public DbSet<UserArchived> UserArchive { get; set; }
    public DbSet<UserSubscriptionArchived> UserSubscriptionArchive { get; set; }
    public DbSet<UserEventSubscriptionArchive> UserEventSubscriptionArchive { get; set; }
    public DbSet<UserEventSubscription> UserEventSubscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Organizator)
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizatorId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasOne(s => s.User).WithMany(u => u.UserSubscriptions).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(s => s.Teacher).WithMany(u => u.Subscribers).HasForeignKey(s => s.TeacherId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserEventSubscription>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.HasOne(s => s.User).WithMany(u => u.EventSubscriptions).HasForeignKey(s => s.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(s => s.Event).WithMany(e => e.Subscribers).HasForeignKey(s => s.EventId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserSubscriptionArchived>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.UserId).IsRequired();
            entity.Property(s => s.TeacherId).IsRequired();
        });

        modelBuilder.Entity<UserEventSubscriptionArchive>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.UserId).IsRequired();
            entity.Property(s => s.EventId).IsRequired();
        });

        modelBuilder.Entity<EventArchived>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.OrganizatorId).IsRequired();
        });
    }

    private void NormalizeDateTimes()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State is not (EntityState.Added or EntityState.Modified))
            {
                continue;
            }

            foreach (var property in entry.Properties)
            {
                if (property.CurrentValue is DateTime dateTime)
                {
                    property.CurrentValue = NormalizeDateTime(dateTime);
                }
            }
        }
    }

    private static DateTime NormalizeDateTime(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}