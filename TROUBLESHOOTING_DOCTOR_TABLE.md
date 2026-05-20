# 🔧 Troubleshooting Doctor Table Missing Error

## Problem
- Getting 500 error when calling GET /api/doctors
- Doctor table doesn't exist in database
- Error message: "An error occurred while retrieving doctors"

## Root Cause
The application code was updated but **not rebuilt**, so the changes haven't been compiled yet.

---

## ✅ Solution - 3 Steps

### STEP 1: Clean Build

**Close the running application first!**

Then run:
```bash
cd c:\Athul\AIDentalTool
dotnet clean
dotnet build
```

Expected output:
```
Build succeeded.
```

### STEP 2: Delete Existing Database (Optional but recommended)

The existing database was created before Doctor entity was added. Delete it to force recreation with all tables:

**Option A: Delete via Visual Studio**
- Open SQL Server Object Explorer
- Find your ClinicOSDb database
- Right-click → Delete

**Option B: Delete via SQL Command**
```sql
DROP DATABASE ClinicOSDb;
```

**Option C: Delete database file (if using LocalDB)**
```
C:\Users\<YourUsername>\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\mssqllocaldb\
```

### STEP 3: Restart Application

```bash
cd c:\Athul\AIDentalTool\ClinicOS.API
dotnet run
```

The application will:
1. Automatically create the database
2. Create all tables (including Doctor table)
3. Seed default data
4. Start normally

---

## 🧪 Verify Success

### Check 1: Doctor Table Created
Open SQL Server Management Studio and run:
```sql
SELECT * FROM Doctors;
```

Expected: Empty table (or table doesn't error)

### Check 2: API Test
Open Swagger UI: `https://localhost:5001`

1. Scroll to **Doctors** section
2. Click **GET /api/doctors**
3. Click **"Try it out"**
4. Click **"Execute"**

Expected Response:
```
Response Code: 200
Response body: []
```

### Check 3: Create Sample Doctor
1. Click **POST /api/doctors**
2. Click **"Try it out"**
3. Paste this JSON:

```json
{
  "userId": 2,
  "specialization": "Orthodontist",
  "licenseNumber": "DEN-2024-001",
  "yearsOfExperience": 8,
  "bio": "Specializes in cosmetic dentistry",
  "consultationFee": 500.00,
  "department": "Orthodontics",
  "clinicLocation": "Main Branch",
  "isAvailable": true
}
```

4. Click **"Execute"**

Expected Response:
```
Response Code: 201
Response body: { doctor object with all fields }
```

---

## 📋 Complete Troubleshooting Checklist

- [ ] Application is stopped (CTRL+C)
- [ ] Ran `dotnet clean`
- [ ] Ran `dotnet build` (successfully)
- [ ] Old database deleted or renamed
- [ ] Application restarted with `dotnet run`
- [ ] Checked SQL Server for Doctor table
- [ ] GET /api/doctors returns 200 OK
- [ ] Can create a doctor successfully
- [ ] Doctor appears when querying

---

## 🔍 If Still Having Issues

### Issue: "Cannot connect to SQL Server"
**Solution**: Check connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=ClinicOSDb;User Id=sa;Password=YourStrong@Password;TrustServerCertificate=True"
}
```

### Issue: "Access Denied" error
**Solution**: Make sure SQL Server is running:
```bash
# For SQL Server Express
sqlservr.exe -s SQLEXPRESS

# Or use Services.msc to start SQL Server
```

### Issue: Database file locked
**Solution**: Close all connections:
- Close SSMS
- Close Swagger UI
- Restart SQL Server service

### Issue: Still getting 500 error
**Solution**: Check application logs:
```bash
# Look in logs folder
cd c:\Athul\AIDentalTool\ClinicOS.API\logs
# Open latest log file
```

---

## 🚀 Complete Fresh Start (Nuclear Option)

If nothing works, start completely fresh:

```bash
# 1. Stop application
# (Press CTRL+C)

# 2. Clean everything
cd c:\Athul\AIDentalTool
dotnet clean
rmdir bin /s /q
rmdir obj /s /q

# 3. Delete database
# (Use SQL Server Management Studio or command above)

# 4. Restore packages
dotnet restore

# 5. Build fresh
dotnet build

# 6. Run
cd ClinicOS.API
dotnet run
```

---

## 📊 Database Schema Check

After application runs, verify the Doctor table exists with correct columns:

```sql
USE ClinicOSDb;

-- Check if Doctors table exists
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'Doctors';

-- Show all columns in Doctors table
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Doctors'
ORDER BY ORDINAL_POSITION;

-- Show indexes
SELECT * 
FROM sys.indexes 
WHERE object_id = OBJECT_ID('Doctors');

-- Show relationships
SELECT * 
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE 
WHERE TABLE_NAME = 'Doctors' AND CONSTRAINT_NAME LIKE 'FK%';
```

Expected columns:
- Id (int, PK)
- UserId (int, FK to Users)
- Specialization (nvarchar(100))
- LicenseNumber (nvarchar(50), unique)
- YearsOfExperience (int)
- Bio (nvarchar(500), nullable)
- ConsultationFee (decimal)
- Department (nvarchar(100), nullable)
- ClinicLocation (nvarchar(200), nullable)
- IsAvailable (bit)
- CreatedAt (datetime2)
- UpdatedAt (datetime2)
- CreatedBy (nvarchar(200), nullable)
- UpdatedBy (nvarchar(200), nullable)

---

## 💡 Quick Reference Commands

```bash
# Check if build was successful
dotnet build --no-restore

# Run with verbose logging
dotnet run --verbosity=diagnostic

# Check if database is created
sqlcmd -S localhost -U sa -P YourStrong@Password -Q "SELECT name FROM sys.databases WHERE name='ClinicOSDb'"

# View all tables
sqlcmd -S localhost -U sa -P YourStrong@Password -d ClinicOSDb -Q "SELECT name FROM sys.objects WHERE type='U'"
```

---

## ✨ Once Working

After successful setup:

1. ✅ Doctor table exists in database
2. ✅ GET /api/doctors returns 200 OK
3. ✅ Can create doctors via POST
4. ✅ Can retrieve doctors by various filters
5. ✅ All authorization working
6. ✅ Ready for integration with other modules

---

**Need more help?** Check the logs in: `c:\Athul\AIDentalTool\ClinicOS.API\logs\`
