using ClinicOS.Application.Common;
using ClinicOS.Domain.Entities;
using ClinicOS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await RbacSeeder.SeedAsync(context);

        Clinic? demoClinic = await context.Clinics.FirstOrDefaultAsync(c => c.Code == "demo-dental");
        if (demoClinic == null)
        {
            demoClinic = new Clinic
            {
                Code = "demo-dental",
                Name = "Demo Dental Clinic",
                Address = "123 Dental Street",
                PhoneNumber = "+911234567890",
                Email = "contact@demodental.com",
                City = "Mumbai",
                State = "Maharashtra",
                Country = "India",
                IsActive = true,
                CreatedBy = "System"
            };
            context.Clinics.Add(demoClinic);
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync(u => u.Username == "superadmin" && u.ClinicId == null))
        {
            var superAdmin = new User
            {
                Username = "superadmin",
                PasswordHash = PasswordHasher.Hash("SuperAdmin@123"),
                FullName = "Platform Super Admin",
                Email = "superadmin@clinicos.com",
                PhoneNumber = "+10000000001",
                ClinicId = null,
                IsActive = true,
                CreatedBy = "System"
            };
            context.Users.Add(superAdmin);
            await context.SaveChangesAsync();
            await RbacSeeder.AssignRoleIfMissingAsync(context, superAdmin.Id, RoleNames.SuperAdmin);
        }

        if (!await context.Users.AnyAsync(u => u.Username == "admin" && u.ClinicId == demoClinic.Id))
        {
            var admin = new User
            {
                ClinicId = demoClinic.Id,
                Username = "admin",
                PasswordHash = PasswordHasher.Hash("Admin@123"),
                FullName = "Clinic Administrator",
                Email = "admin@demodental.com",
                PhoneNumber = "+911234567891",
                IsActive = true,
                CreatedBy = "System"
            };
            context.Users.Add(admin);
            await context.SaveChangesAsync();
            await RbacSeeder.AssignRoleIfMissingAsync(context, admin.Id, RoleNames.Admin);
        }

        if (!await context.Users.AnyAsync(u => u.Username == "doctor1" && u.ClinicId == demoClinic.Id))
        {
            var doctorUser = new User
            {
                ClinicId = demoClinic.Id,
                Username = "doctor1",
                PasswordHash = PasswordHasher.Hash("Doctor@123"),
                FullName = "Dr. Sarah Smith",
                Email = "doctor1@demodental.com",
                PhoneNumber = "+911234567892",
                IsActive = true,
                CreatedBy = "System"
            };
            context.Users.Add(doctorUser);
            await context.SaveChangesAsync();
            await RbacSeeder.AssignRoleIfMissingAsync(context, doctorUser.Id, RoleNames.Doctor);

            context.Doctors.Add(new Doctor
            {
                ClinicId = demoClinic.Id,
                UserId = doctorUser.Id,
                Specialization = "General Dentistry",
                LicenseNumber = "DENT-DEMO-001",
                YearsOfExperience = 10,
                ConsultationFee = 500,
                Department = "General",
                IsAvailable = true,
                CreatedBy = "System"
            });
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync(u => u.Username == "reception1" && u.ClinicId == demoClinic.Id))
        {
            var reception = new User
            {
                ClinicId = demoClinic.Id,
                Username = "reception1",
                PasswordHash = PasswordHasher.Hash("Reception@123"),
                FullName = "Jane Reception",
                Email = "reception1@demodental.com",
                PhoneNumber = "+911234567893",
                IsActive = true,
                CreatedBy = "System"
            };
            context.Users.Add(reception);
            await context.SaveChangesAsync();
            await RbacSeeder.AssignRoleIfMissingAsync(context, reception.Id, RoleNames.Receptionist);
        }

        if (!await context.Patients.AnyAsync(p => p.ClinicId == demoClinic.Id))
        {
            context.Patients.AddRange(
                new Patient
                {
                    ClinicId = demoClinic.Id,
                    PatientCode = "P001",
                    FullName = "Alice Johnson",
                    Gender = Gender.Female,
                    DateOfBirth = new DateTime(1985, 5, 15),
                    PhoneNumber = "+911234567894",
                    Email = "alice.johnson@email.com",
                    Address = "123 Main St",
                    BloodGroup = "O+",
                    CreatedBy = "System"
                },
                new Patient
                {
                    ClinicId = demoClinic.Id,
                    PatientCode = "P002",
                    FullName = "Bob Williams",
                    Gender = Gender.Male,
                    DateOfBirth = new DateTime(1990, 8, 22),
                    PhoneNumber = "+911234567895",
                    Email = "bob.williams@email.com",
                    Address = "456 Oak Ave",
                    BloodGroup = "A+",
                    MedicalHistory = "Hypertension",
                    Allergies = "Penicillin",
                    CreatedBy = "System"
                });
            await context.SaveChangesAsync();
        }
    }
}
