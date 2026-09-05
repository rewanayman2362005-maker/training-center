using Microsoft.EntityFrameworkCore;
using TrainingCenter.Models;

namespace TrainingCenter.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Trainee> Trainees => Set<Trainee>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Trainee)
            .WithMany()
            .HasForeignKey(b => b.TraineeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Course)
            .WithMany()
            .HasForeignKey(b => b.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
