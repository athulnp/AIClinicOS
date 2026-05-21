using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Common;
using ClinicOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Data;

/// <summary>
/// Application database context with multi-clinic tenant query filters.
/// </summary>
public class AppDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext) : base(options)
    {
        _tenantContext = tenantContext;
    }

    public DbSet<Clinic> Clinics { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Billing> Billings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserRoleAssignment> UserRoleAssignments { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Reminder> Reminders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Clinic>()
            .HasIndex(c => c.Code)
            .IsUnique();

        modelBuilder.Entity<Patient>().HasQueryFilter(p =>
            !p.IsDeleted && (!_tenantContext.HasClinic || p.ClinicId == _tenantContext.ClinicId));

        modelBuilder.Entity<Patient>()
            .HasIndex(p => new { p.ClinicId, p.PatientCode })
            .IsUnique();

        modelBuilder.Entity<Patient>()
            .HasIndex(p => new { p.ClinicId, p.PhoneNumber });

        modelBuilder.Entity<User>().HasQueryFilter(u =>
            !_tenantContext.HasClinic || u.ClinicId == _tenantContext.ClinicId);

        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.ClinicId, u.Username })
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => new { u.ClinicId, u.Email })
            .IsUnique();

        modelBuilder.Entity<Permission>()
            .HasIndex(p => p.Code)
            .IsUnique();

        modelBuilder.Entity<Role>()
            .HasIndex(r => new { r.Name, r.ClinicId })
            .IsUnique()
            .HasFilter("[ClinicId] IS NOT NULL");

        modelBuilder.Entity<Role>()
            .HasIndex(r => r.Name)
            .IsUnique()
            .HasFilter("[ClinicId] IS NULL");

        modelBuilder.Entity<RolePermission>()
            .HasKey(rp => new { rp.RoleId, rp.PermissionId });

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePermissions)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RolePermission>()
            .HasOne(rp => rp.Permission)
            .WithMany(p => p.RolePermissions)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRoleAssignment>()
            .HasKey(ura => new { ura.UserId, ura.RoleId });

        modelBuilder.Entity<UserRoleAssignment>()
            .HasOne(ura => ura.User)
            .WithMany(u => u.UserRoleAssignments)
            .HasForeignKey(ura => ura.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserRoleAssignment>()
            .HasOne(ura => ura.Role)
            .WithMany(r => r.UserAssignments)
            .HasForeignKey(ura => ura.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Role>()
            .HasOne(r => r.Clinic)
            .WithMany()
            .HasForeignKey(r => r.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Appointment>().HasQueryFilter(a =>
            !_tenantContext.HasClinic || a.ClinicId == _tenantContext.ClinicId);

        modelBuilder.Entity<Appointment>()
            .HasIndex(a => new { a.ClinicId, a.DoctorId, a.AppointmentDate, a.StartTime });

        modelBuilder.Entity<Billing>().HasQueryFilter(b =>
            !_tenantContext.HasClinic || b.ClinicId == _tenantContext.ClinicId);

        modelBuilder.Entity<Billing>()
            .HasIndex(b => new { b.ClinicId, b.InvoiceNumber })
            .IsUnique();

        modelBuilder.Entity<Doctor>().HasQueryFilter(d =>
            !_tenantContext.HasClinic || d.ClinicId == _tenantContext.ClinicId);

        modelBuilder.Entity<Doctor>()
            .HasIndex(d => new { d.ClinicId, d.LicenseNumber })
            .IsUnique();

        modelBuilder.Entity<Reminder>().HasQueryFilter(r =>
            !_tenantContext.HasClinic || r.ClinicId == _tenantContext.ClinicId);

        modelBuilder.Entity<Clinic>()
            .HasMany(c => c.Users)
            .WithOne(u => u.Clinic)
            .HasForeignKey(u => u.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Clinic>()
            .HasMany(c => c.Patients)
            .WithOne(p => p.Clinic)
            .HasForeignKey(p => p.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.User)
            .WithOne(u => u.DoctorDetails)
            .HasForeignKey<Doctor>(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);

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

        // SQL Server: no cascade from Clinic — avoids multiple cascade paths (e.g. Clinic→Appointment→Reminder vs Clinic→Reminder)
        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Clinic)
            .WithMany()
            .HasForeignKey(a => a.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Billing>()
            .HasOne(b => b.Clinic)
            .WithMany()
            .HasForeignKey(b => b.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Doctor>()
            .HasOne(d => d.Clinic)
            .WithMany()
            .HasForeignKey(d => d.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Reminder>()
            .HasOne(r => r.Clinic)
            .WithMany()
            .HasForeignKey(r => r.ClinicId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public override int SaveChanges()
    {
        ApplyTenantOnInsert();
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyTenantOnInsert();
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyTenantOnInsert()
    {
        if (!_tenantContext.HasClinic)
            return;

        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State == EntityState.Added && entry.Entity.ClinicId == 0)
            {
                entry.Entity.ClinicId = _tenantContext.ClinicId!.Value;
            }
        }
    }

    private void UpdateTimestamps()
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = DateTime.UtcNow;

            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}
