# Doctor Details Implementation Guide

## 📋 Overview
This document outlines the implementation of a dedicated Doctor Details table in the ClinicOS system. Previously, doctor information was only stored in the User table with a "Doctor" role. Now doctors have a dedicated entity with comprehensive professional information.

## 🏗️ Architecture Changes

### New Entity: Doctor
**Location:** `ClinicOS.Domain\Entities\Doctor.cs`

**Fields:**
- `Id` (Primary Key)
- `UserId` (Foreign Key to User - One-to-One relationship)
- `Specialization` (required) - e.g., "Orthodontist", "Pediatric Dentist"
- `LicenseNumber` (required, unique) - Medical license number
- `YearsOfExperience` (required) - Total years of professional experience
- `Bio` (optional) - Professional biography
- `ConsultationFee` (required) - Cost per appointment (decimal)
- `Department` (optional) - Department assignment
- `ClinicLocation` (optional) - Specific clinic location
- `IsAvailable` (required, default=true) - Availability status
- `CreatedAt` (audit field)
- `UpdatedAt` (audit field)

**Relationships:**
- **One-to-One** with User table
  - A User with "Doctor" role can have one Doctor profile
  - A Doctor profile belongs to exactly one User

### Updated User Entity
**File:** `ClinicOS.Domain\Entities\User.cs`

Added navigation property:
```csharp
public virtual Doctor? DoctorDetails { get; set; }
```

---

## 📁 Files Created

### 1. Domain Layer
- `ClinicOS.Domain\Entities\Doctor.cs` - Core entity definition

### 2. Application Layer
**DTOs:**
- `ClinicOS.Application\DTOs\CreateDoctorDto.cs` - For creating new doctor profiles
- `ClinicOS.Application\DTOs\UpdateDoctorDto.cs` - For updating doctor information
- `ClinicOS.Application\DTOs\DoctorResponseDto.cs` - Response DTO with user details

**Validators:**
- `ClinicOS.Application\Validators\CreateDoctorValidator.cs`
- `ClinicOS.Application\Validators\UpdateDoctorValidator.cs`

**Interfaces:**
- `ClinicOS.Application\Interfaces\IDoctorRepository.cs`
- `ClinicOS.Application\Interfaces\IDoctorService.cs`

**Services:**
- `ClinicOS.Application\Services\DoctorService.cs` - Business logic implementation

**Mapping:**
- Updated `ClinicOS.Application\Mapping\MappingProfile.cs` - Added Doctor mappings

### 3. Infrastructure Layer
- `ClinicOS.Infrastructure\Repositories\DoctorRepository.cs` - Data access implementation

### 4. API Layer
- `ClinicOS.API\Controllers\DoctorsController.cs` - RESTful endpoints

### 5. Configuration
- Updated `ClinicOS.Infrastructure\Data\AppDbContext.cs` - Added Doctor DbSet and relationships
- Updated `ClinicOS.API\Program.cs` - Registered services and repositories

---

## 🔌 API Endpoints

### Get Endpoints
```
GET /api/doctors                          - Get all doctors
GET /api/doctors/{id}                     - Get doctor by ID
GET /api/doctors/user/{userId}            - Get doctor by User ID
GET /api/doctors/license/{licenseNumber}  - Get doctor by license number
GET /api/doctors/specialization/{spec}    - Get doctors by specialization
GET /api/doctors/available                - Get available doctors
```

### Mutation Endpoints
```
POST   /api/doctors                       - Create doctor profile (Admin only)
PUT    /api/doctors/{id}                  - Update doctor profile (Admin/Doctor)
DELETE /api/doctors/{id}                  - Delete doctor profile (Admin only)
```

---

## 📝 Request/Response Examples

### Create Doctor
**Request:**
```json
POST /api/doctors

{
  "userId": 2,
  "specialization": "Orthodontist",
  "licenseNumber": "DEN-2024-001",
  "yearsOfExperience": 8,
  "bio": "Specialized in cosmetic dentistry",
  "consultationFee": 500.00,
  "department": "Orthodontics",
  "clinicLocation": "Main Branch",
  "isAvailable": true
}
```

**Response (201 Created):**
```json
{
  "id": 1,
  "userId": 2,
  "fullName": "Dr. John Smith",
  "email": "john@clinicos.com",
  "phoneNumber": "+1-555-1234",
  "specialization": "Orthodontist",
  "licenseNumber": "DEN-2024-001",
  "yearsOfExperience": 8,
  "bio": "Specialized in cosmetic dentistry",
  "consultationFee": 500.00,
  "department": "Orthodontics",
  "clinicLocation": "Main Branch",
  "isAvailable": true,
  "createdAt": "2026-05-20T10:30:00Z",
  "updatedAt": "2026-05-20T10:30:00Z"
}
```

### Update Doctor
**Request:**
```json
PUT /api/doctors/1

{
  "consultationFee": 600.00,
  "department": "Advanced Orthodontics",
  "isAvailable": false
}
```

---

## ✅ Validations

### CreateDoctorValidator
- `UserId`: Required, valid user ID > 0
- `Specialization`: Required, max 100 chars
- `LicenseNumber`: Required, max 50 chars, format: `^[A-Z0-9\-]+$`
- `YearsOfExperience`: ≥ 0, ≤ 70
- `Bio`: Optional, max 500 chars
- `ConsultationFee`: Must be > 0
- `Department`: Optional, max 100 chars
- `ClinicLocation`: Optional, max 200 chars

### UpdateDoctorValidator
- All fields optional
- Same validation rules apply when provided
- Unique license number check (excludes current record)

---

## 🔐 Authorization

| Endpoint | Method | Role(s) |
|----------|--------|---------|
| GET /api/doctors/* | All GET | All Authenticated |
| POST /api/doctors | POST | Admin |
| PUT /api/doctors/{id} | PUT | Admin, Doctor |
| DELETE /api/doctors/{id} | DELETE | Admin |

---

## 🗄️ Database Migration

### Run Migration
```bash
cd ClinicOS.Infrastructure
dotnet ef migrations add AddDoctorEntity --startup-project ../ClinicOS.API
dotnet ef database update --startup-project ../ClinicOS.API
```

### Changes Made
1. Created `Doctor` table
2. Added unique index on `LicenseNumber`
3. Created Foreign Key from `Doctor.UserId` to `User.Id`
4. Added audit columns (`CreatedAt`, `UpdatedAt`)

---

## 🔗 Relationships

```
User (One)
  ├── Id (PK)
  ├── Username
  ├── FullName
  ├── Email
  └── Role = "Doctor"
      └─ DoctorDetails ──(1:1)─→ Doctor (One)
                                   ├── Id (PK)
                                   ├── UserId (FK)
                                   ├── Specialization
                                   ├── LicenseNumber (Unique)
                                   ├── YearsOfExperience
                                   ├── ConsultationFee
                                   └── ...
```

---

## 🎯 Business Logic

### Doctor Creation
1. Validate input using `CreateDoctorValidator`
2. Verify User exists and has "Doctor" role
3. Verify no existing Doctor profile for this User
4. Verify License Number is unique
5. Create Doctor record
6. Save changes

### Doctor Update
1. Validate input using `UpdateDoctorValidator`
2. Verify Doctor exists
3. If License Number changed, verify uniqueness
4. Update only provided fields
5. Save changes

### Doctor Deletion
1. Verify Doctor exists
2. Delete Doctor record
3. User account remains (with Doctor role but no profile)

---

## 📊 Database Schema

```sql
CREATE TABLE Doctors (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId INT NOT NULL UNIQUE,
    Specialization NVARCHAR(100) NOT NULL,
    LicenseNumber NVARCHAR(50) NOT NULL UNIQUE,
    YearsOfExperience INT NOT NULL,
    Bio NVARCHAR(500) NULL,
    ConsultationFee DECIMAL(10,2) NOT NULL,
    Department NVARCHAR(100) NULL,
    ClinicLocation NVARCHAR(200) NULL,
    IsAvailable BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(200) NULL,
    UpdatedBy NVARCHAR(200) NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE RESTRICT,
    UNIQUE(LicenseNumber)
);

CREATE INDEX IX_Doctors_LicenseNumber ON Doctors(LicenseNumber);
CREATE INDEX IX_Doctors_Specialization ON Doctors(Specialization);
CREATE INDEX IX_Doctors_UserId ON Doctors(UserId);
```

---

## 🚀 Usage Example

### Create New Doctor
1. First, create a User with "Doctor" role via `/api/auth/register` or admin panel
2. Then, create Doctor profile:

```bash
curl -X POST http://localhost:5000/api/doctors \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d '{
    "userId": 2,
    "specialization": "Pediatric Dentist",
    "licenseNumber": "DEN-2024-002",
    "yearsOfExperience": 5,
    "bio": "Specializes in children dentistry",
    "consultationFee": 400,
    "department": "Pediatrics",
    "clinicLocation": "Branch Office",
    "isAvailable": true
  }'
```

### Get Doctor Details
```bash
curl -X GET http://localhost:5000/api/doctors/1 \
  -H "Authorization: Bearer <token>"
```

### Get Available Doctors for Appointments
```bash
curl -X GET http://localhost:5000/api/doctors/available \
  -H "Authorization: Bearer <token>"
```

---

## 🔄 Integration with Other Modules

### Appointments
- When creating appointments, fetch available doctors from the new Doctor endpoint
- Use consultation fee from Doctor profile for billing

### Billing
- Reference Doctor's consultation fee in invoice creation
- Include doctor specialization in billing records if needed

### Future Enhancements
- Add schedule/working hours to Doctor entity
- Add qualifications/certifications collection
- Add patient ratings/reviews for doctors
- Add availability calendar

---

## 🧪 Testing Checklist

- [ ] Create Doctor profile for existing Doctor user
- [ ] Update Doctor consultation fee
- [ ] Get all doctors
- [ ] Get doctors by specialization
- [ ] Get available doctors
- [ ] Verify license number uniqueness
- [ ] Verify one-to-one relationship
- [ ] Delete doctor profile
- [ ] Authorization checks (Admin/Doctor roles)
- [ ] Validation error handling
- [ ] Database migration and rollback

---

## 📌 Notes

- License numbers must be unique across all doctors
- A User must exist with "Doctor" role before creating Doctor profile
- Deleting a Doctor profile doesn't delete the User account
- Doctor profile is optional for Doctor users (allows for future use)
- All dates are in UTC format
- Soft deletes not implemented for Doctor (hard delete only)
