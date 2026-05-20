using ClinicOS.Domain.Entities;
using ClinicOS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Data;

/// <summary>
/// Database seed data
/// </summary>
public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Seed default admin user
        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
        if (adminUser == null)
        {
            adminUser = new User
            {
                Username = "admin",
                PasswordHash = HashPassword("Admin@123"),
                FullName = "System Administrator",
                Email = "admin@clinicos.com",
                PhoneNumber = "+1234567890",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedBy = "System"
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }

        // Seed sample patients
        if (!context.Patients.Any())
        {
            var patient1 = new Patient
            {
                PatientCode = "P001",
                FullName = "Alice Johnson",
                Gender = Gender.Female,
                DateOfBirth = new DateTime(1985, 5, 15),
                PhoneNumber = "+1234567893",
                Email = "alice.johnson@email.com",
                Address = "123 Main St, City",
                BloodGroup = "O+",
                MedicalHistory = "No significant medical history",
                Allergies = "None",
                EmergencyContact = "+1234567894",
                Notes = "Regular patient",
                CreatedBy = "System"
            };

            context.Patients.Add(patient1);

            var patient2 = new Patient
            {
                PatientCode = "P002",
                FullName = "Bob Williams",
                Gender = Gender.Male,
                DateOfBirth = new DateTime(1990, 8, 22),
                PhoneNumber = "+1234567895",
                Email = "bob.williams@email.com",
                Address = "456 Oak Ave, Town",
                BloodGroup = "A+",
                MedicalHistory = "Hypertension",
                Allergies = "Penicillin",
                EmergencyContact = "+1234567896",
                Notes = "New patient",
                CreatedBy = "System"
            };

            context.Patients.Add(patient2);

            await context.SaveChangesAsync();
        }
    }

    private static string HashPassword(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
