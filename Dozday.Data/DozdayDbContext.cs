using Dozday.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Dozday.Data;

public class DozdayDbContext : DbContext
{
    public DozdayDbContext(DbContextOptions<DozdayDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserSubscription> UserSubscriptions { get; set; }

    public DbSet<QualityAssurance> QualityAssurances { get; set; }

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

        modelBuilder.Entity<Event>(entity => { entity.HasKey(e => e.Id); });

        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.HasKey(s => s.Id);

            entity.HasOne(s => s.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(s => s.Teacher)
                .WithMany(u => u.Subscribers)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}