using Microsoft.EntityFrameworkCore;
using QMS.DTO;

namespace QMS.Db;

public class QmsDbContext : DbContext
{
    public DbSet<FrontDeskTerminal> FrontDeskTerminals { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketCounter> TicketCounters { get; set; }
    public QmsDbContext(DbContextOptions<QmsDbContext> options)
       : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FrontDeskTerminal>()
            .HasKey(f => f.DeviceId);
            

        modelBuilder.Entity<FrontDeskTerminal>()
            .Property(f => f.DeviceId)
            .HasMaxLength(191)
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
            .HasIndex(t => t.TicketNumber)
            .IsUnique();
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

        modelBuilder.Entity<TicketCounter>()
            .HasKey(tc => tc.Id);
        modelBuilder.Entity<TicketCounter>()
            .Property(tc => tc.Id)
            .ValueGeneratedNever(); // Disable auto-increment, we'll set the Id manually
        modelBuilder.Entity<TicketCounter>()
            .Property(tc => tc.CurrentNumber)
            .IsRequired();
        modelBuilder.Entity<TicketCounter>()
            .Property(tc => tc.LastUpdated)
            .IsRequired();

    }
}
