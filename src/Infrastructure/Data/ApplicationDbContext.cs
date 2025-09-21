using EventTicketing.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace EventTicketing.Infrastructure.Data
{
    /// <summary>
    /// Contexto de base de datos para la aplicación
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Seat> Seats { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de entidades
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.Location).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Category).HasMaxLength(50);
                entity.Property(e => e.BasePrice).HasColumnType("decimal(18,2)");
                
                // Relación con Tickets
                entity.HasMany(e => e.IssuedTickets)
                      .WithOne()
                      .HasForeignKey(t => t.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
                
                // Relación con Seats
                entity.HasMany(e => e.Seats)
                      .WithOne()
                      .HasForeignKey(s => s.EventId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.TicketCode).IsRequired().HasMaxLength(20);
                entity.Property(t => t.Price).HasColumnType("decimal(18,2)");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.PhoneNumber).HasMaxLength(20);
                
                // Relación con Tickets
                entity.HasMany(u => u.Tickets)
                      .WithOne()
                      .HasForeignKey(t => t.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Row).IsRequired().HasMaxLength(10);
                entity.Property(s => s.Section).IsRequired().HasMaxLength(20);
                entity.Property(s => s.PriceMultiplier).HasColumnType("decimal(18,2)");
            });
        }
    }
}