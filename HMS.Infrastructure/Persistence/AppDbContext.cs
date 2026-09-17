using HMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace HMS.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationRoom> ReservationRooms { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ======================= Hotel Configuration =======================
            modelBuilder.Entity<Hotel>(entity =>
            {
                entity.HasKey(e => e.HotelId);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.Property(e => e.Rating)
                    .HasPrecision(3, 1);
            });

            // ======================= Manager Configuration =======================
            modelBuilder.Entity<Manager>(entity =>
            {
                entity.HasKey(e => e.ManagerId);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PersonalNumber)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.EmployeeId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50);

                // Unique Constraints
                entity.HasIndex(e => e.PersonalNumber)
                    .IsUnique();

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.HasIndex(e => e.EmployeeId)
                    .IsUnique();

                // Foreign Key: Manager -> Hotel (1:M)
                entity.HasOne(e => e.Hotel)
                    .WithMany(h => h.Managers)
                    .HasForeignKey(e => e.HotelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // ======================= Room Configuration =======================
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.RoomId);

                entity.Property(e => e.RoomNumber)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                // Decimal Precision: 10 digits total, 2 decimal places
                entity.Property(e => e.PricePerNight)
                    .HasPrecision(10, 2);

                // Unique constraint on Room Number per Hotel
                entity.HasIndex(e => new { e.HotelId, e.RoomNumber })
                    .IsUnique();

                // Foreign Key: Room -> Hotel (1:M)
                entity.HasOne(e => e.Hotel)
                    .WithMany(h => h.Rooms)
                    .HasForeignKey(e => e.HotelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ======================= Guest Configuration =======================
            modelBuilder.Entity<Guest>(entity =>
            {
                entity.HasKey(e => e.GuestId);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Address)
                    .HasMaxLength(200);

                entity.Property(e => e.City)
                    .HasMaxLength(50);

                entity.Property(e => e.Country)
                    .HasMaxLength(50);

                // Unique Constraints
                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.HasIndex(e => e.PhoneNumber)
                    .IsUnique();
            });

            // ======================= Reservation Configuration =======================
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.ReservationId);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.SpecialRequests)
                    .HasMaxLength(500);

                entity.Property(e => e.TotalPrice)
                    .HasPrecision(12, 2);

                // Foreign Key: Reservation -> Guest (M:1)
                entity.HasOne(e => e.Guest)
                    .WithMany(g => g.Reservations)
                    .HasForeignKey(e => e.GuestId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Foreign Key: Reservation -> Hotel (M:1)
                entity.HasOne(e => e.Hotel)
                    .WithMany(h => h.Reservations)
                    .HasForeignKey(e => e.HotelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ======================= ReservationRoom Configuration =======================
            modelBuilder.Entity<ReservationRoom>(entity =>
            {
                entity.HasKey(e => e.ReservationRoomId);

                entity.Property(e => e.PricePerNight)
                    .HasPrecision(10, 2);

                // Foreign Key: ReservationRoom -> Reservation
                entity.HasOne(e => e.Reservation)
                    .WithMany(r => r.ReservationRooms)
                    .HasForeignKey(e => e.ReservationId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Foreign Key: ReservationRoom -> Room
                entity.HasOne(e => e.Room)
                    .WithMany(r => r.ReservationRooms)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Composite unique index for Reservation-Room pairs
                entity.HasIndex(e => new { e.ReservationId, e.RoomId })
                    .IsUnique();
            });

            // ======================= User Configuration =======================
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.PasswordHash)
                    .IsRequired();

                entity.Property(e => e.Role)
                    .IsRequired()
                    .HasMaxLength(50);

                // Unique constraints
                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.HasIndex(e => e.PhoneNumber)
                    .IsUnique();

                // Foreign Keys
                entity.HasOne(e => e.Manager)
                    .WithMany()
                    .HasForeignKey(e => e.ManagerId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.Guest)
                    .WithMany()
                    .HasForeignKey(e => e.GuestId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

        }
    }
}
   
