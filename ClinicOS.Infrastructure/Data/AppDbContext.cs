using ClinicOS.Domain.Common;
using ClinicOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Data;

/// <summary>
/// Application database context
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Billing> Billings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Reminder> Reminders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure soft delete query filter globally
        modelBuilder.Entity<Patient>().HasQueryFilter(p => !p.IsDeleted);

        // Configure indexes
        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.PatientCode)
            .IsUnique();

        modelBuilder.Entity<Patient>()
            .HasIndex(p => p.PhoneNumber);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Billing>()
            .HasIndex(b => b.InvoiceNumber)
            .IsUnique();

        modelBuilder.Entity<Appointment>()
            .HasIndex(a => new { a.DoctorId, a.AppointmentDate, a.StartTime });

        // Configure Doctor entity relationships
        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.User)
            .WithOne(u => u.DoctorDetails)
            .HasForeignKey<Doctor>(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Doctor>()
            .HasIndex(d => d.LicenseNumber)
            .IsUnique();

        // Configure relationships
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Doctor)
            .WithMany(u => u.DoctorAppointments)
            .HasForeignKey(a => a.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Billing>()
            .HasOne(b => b.Patient)
            .WithMany(p => p.Billings)
            .HasForeignKey(b => b.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Billing>()
            .HasOne(b => b.Appointment)
            .WithMany(a => a.Billings)
            .HasForeignKey(b => b.AppointmentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reminder>()
            .HasOne(r => r.Appointment)
            .WithMany(a => a.Reminders)
            .HasForeignKey(r => r.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
