using Booking.Entities;
using Microsoft.EntityFrameworkCore;

namespace Booking.DataAccess;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // un user a plusieurs réservations
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // une salle a plusieurs réservations
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Room)
            .WithMany(r => r.Reservations)
            .HasForeignKey(r => r.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Room>().HasIndex(r => r.Name).IsUnique();
    }
}
