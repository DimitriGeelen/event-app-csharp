using EventApp.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace EventApp.Web.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Event> Events { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Event>(entity =>
        {
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.StartDateTime)
                .IsRequired();

            entity.Property(e => e.EndDateTime)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            entity.Property(e => e.Location)
                .HasMaxLength(200);

            entity.Property(e => e.Address)
                .HasMaxLength(500);

            entity.Property(e => e.ImagePath)
                .HasMaxLength(1000);

            entity.Property(e => e.DocumentPath)
                .HasMaxLength(1000);
        });
    }
}