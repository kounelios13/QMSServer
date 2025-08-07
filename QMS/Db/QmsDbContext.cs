using Microsoft.EntityFrameworkCore;
using QMS.DTO;

namespace QMS.Db;

public class QmsDbContext : DbContext
{
    public DbSet<FrontDeskTerminal> FrontDeskTerminals { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=qms.db"); // Ensure the Microsoft.EntityFrameworkCore.Sqlite package is installed  
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FrontDeskTerminal>()
            .HasKey(f => f.DeviceId);

        modelBuilder.Entity<FrontDeskTerminal>()
            .Property(f => f.DeviceId)
            .IsRequired();

        modelBuilder.Entity<FrontDeskTerminal>()
            .Property(f => f.LastSeen)
            .IsRequired();

        modelBuilder.Entity<FrontDeskTerminal>()
            .Property(f => f.IPAddress)
            .HasMaxLength(45);

        modelBuilder.Entity<FrontDeskTerminal>()
            .Property(f => f.DeviceName)
            .HasMaxLength(100);
        modelBuilder.Entity<Ticket>()
            .HasKey(t => t.Id);
        modelBuilder.Entity<Ticket>()
            .Property(t => t.TicketNumber)
            .IsRequired()
            .HasMaxLength(50);
        modelBuilder.Entity<Ticket>()
            .Property(t => t.Status)
            .HasDefaultValue(TicketStatus.Pending);
        modelBuilder.Entity<Ticket>()
            .Property(t => t.IssuedAt)
            .IsRequired();
        modelBuilder.Entity<Ticket>()
            .Property(t => t.IPAddress)
            .HasMaxLength(45);
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.FrontDeskTerminal)
            .WithMany()
            .HasForeignKey(t => t.FrontDeskTerminalId)
            .IsRequired(false); // Allow null for unassigned terminals

    }
}
