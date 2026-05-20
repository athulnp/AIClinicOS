## 🎓 Doctor Details - Quick Start Guide

### Overview
The Doctor Details feature adds a dedicated `Doctor` table to store professional information for users with the "Doctor" role. Each doctor user can have exactly one doctor profile.

---

## 📊 Entity Relationships

```
User (Role = "Doctor")
    ↓ (One-to-One)
Doctor Profile
    ├── License Number (Unique)
    ├── Specialization
    ├── Consultation Fee
    └── Availability Status
```

---

## 🚀 Quick Start

### 1. Run Database Migration

```bash
cd ClinicOS.Infrastructure
dotnet ef migrations add AddDoctorEntity --startup-project ../ClinicOS.API
dotnet ef database update --startup-project ../ClinicOS.API
```

### 2. Start the Application

```bash
cd ClinicOS.API
dotnet run
```

Open: `https://localhost:5001`

### 3. Authenticate

Use default credentials:
```
Username: admin
Password: Admin@123
```

Get JWT token from: `POST /api/auth/login`

---

## 📝 API Examples

### Create Doctor Profile

**Request:**
```bash
curl -X POST https://localhost:5001/api/doctors \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "userId": 2,
    "specialization": "Orthodontist",
    "licenseNumber": "DEN-2024-001",
    "yearsOfExperience": 8,
    "bio": "Board-certified orthodontist with 8 years of experience",
    "consultationFee": 500.00,
    "department": "Orthodontics",
    "clinicLocation": "Main Branch",
    "isAvailable": true
  }'
```

**Response (201 Created):**
```json
{
  "id": 1,
  "userId": 2,
  "fullName": "Dr. John Smith",
  "email": "doctor1@clinicos.com",
  "phoneNumber": "+1-555-0001",
  "specialization": "Orthodontist",
  "licenseNumber": "DEN-2024-001",
  "yearsOfExperience": 8,
  "bio": "Board-certified orthodontist with 8 years of experience",
  "consultationFee": 500.00,
  "department": "Orthodontics",
  "clinicLocation": "Main Branch",
  "isAvailable": true,
  "createdAt": "2026-05-20T10:30:45Z",
  "updatedAt": "2026-05-20T10:30:45Z"
}
```

---

### Get All Doctors

**Request:**
```bash
curl -X GET https://localhost:5001/api/doctors \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Response:**
```json
[
  {
    "id": 1,
    "userId": 2,
    "fullName": "Dr. John Smith",
    "email": "doctor1@clinicos.com",
    "phoneNumber": "+1-555-0001",
    "specialization": "Orthodontist",
    "licenseNumber": "DEN-2024-001",
    "yearsOfExperience": 8,
    "consultationFee": 500.00,
    "isAvailable": true,
    ...
  },
  {
    "id": 2,
    "userId": 3,
    "fullName": "Dr. Sarah Johnson",
    "email": "doctor2@clinicos.com",
    "phoneNumber": "+1-555-0002",
    "specialization": "Pediatric Dentist",
    "licenseNumber": "DEN-2024-002",
    "yearsOfExperience": 5,
    "consultationFee": 400.00,
    "isAvailable": true,
    ...
  }
]
```

---

### Get Available Doctors

**Request:**
```bash
curl -X GET https://localhost:5001/api/doctors/available \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

### Get Doctors by Specialization

**Request:**
```bash
curl -X GET "https://localhost:5001/api/doctors/specialization/Orthodontist" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

### Get Doctor by License Number

**Request:**
```bash
curl -X GET "https://localhost:5001/api/doctors/license/DEN-2024-001" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

### Update Doctor Profile

**Request:**
```bash
curl -X PUT https://localhost:5001/api/doctors/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "consultationFee": 600.00,
    "isAvailable": false
  }'
```

**Response:**
```json
{
  "id": 1,
  "userId": 2,
  "fullName": "Dr. John Smith",
  "consultationFee": 600.00,
  "isAvailable": false,
  ...
}
```

---

### Delete Doctor Profile

**Request:**
```bash
curl -X DELETE https://localhost:5001/api/doctors/1 \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

**Response:** `204 No Content`

---

## 📋 Request Payload Details

### CreateDoctorDto

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| userId | int | ✓ | Must be valid User ID with Doctor role |
| specialization | string | ✓ | Max 100 chars (e.g., "Orthodontist", "Pediatric Dentist") |
| licenseNumber | string | ✓ | Unique, max 50 chars, format: `[A-Z0-9-]` |
| yearsOfExperience | int | ✓ | Range 0-70 |
| bio | string | ✗ | Max 500 chars, optional professional biography |
| consultationFee | decimal | ✓ | Must be > 0 |
| department | string | ✗ | Max 100 chars |
| clinicLocation | string | ✗ | Max 200 chars |
| isAvailable | bool | ✓ | Default: true |

### UpdateDoctorDto

All fields are optional. Only provide fields you want to update.

---

## 🔐 Authorization

| Operation | Role Required |
|-----------|---------------|
| GET /api/doctors* | Any authenticated user |
| POST /api/doctors | Admin only |
| PUT /api/doctors | Admin or own Doctor profile |
| DELETE /api/doctors | Admin only |

---

## ✅ Validation Rules

### Specialization
- Required
- 1-100 characters
- Examples: "Orthodontist", "Pediatric Dentist", "Periodontist"

### License Number
- Required
- 1-50 characters
- Format: `^[A-Z0-9\-]+$` (uppercase letters, numbers, hyphens only)
- Must be unique
- Examples: "DEN-2024-001", "DENT-REG-123"

### Years of Experience
- Required
- Must be 0 or greater
- Must be 70 or less

### Consultation Fee
- Required
- Must be greater than 0
- Decimal with up to 2 decimal places
- Examples: 500.00, 400.50

### Bio
- Optional
- Max 500 characters

### Department
- Optional
- Max 100 characters
- Examples: "Orthodontics", "Pediatrics", "Periodontics"

### Clinic Location
- Optional
- Max 200 characters
- Examples: "Main Branch", "Downtown Office", "Mall Location"

---

## 🚨 Error Responses

### 400 Bad Request
```json
{
  "message": "User must have Doctor role"
}
```

### 404 Not Found
```json
{
  "message": "Doctor not found with this license number"
}
```

### 409 Conflict
```json
{
  "message": "License number already exists"
}
```

### 401 Unauthorized
```json
{
  "message": "Authorization required"
}
```

### 403 Forbidden
```json
{
  "message": "Access denied. Admin role required."
}
```

---

## 📱 Integration Examples

### In Appointment Creation
```csharp
// Get available doctor
var availableDoctors = await doctorService.GetAvailableDoctorsAsync();

// Use consultation fee in billing
var doctor = await doctorService.GetDoctorByIdAsync(doctorId);
var consultationFee = doctor.ConsultationFee;
```

### Filter by Specialization
```csharp
// Get all orthodontists
var orthodontists = await doctorService
    .GetDoctorsBySpecializationAsync("Orthodontist");
```

---

## 🧪 Testing Scenarios

### Scenario 1: Create Multiple Doctors
1. Login as Admin
2. POST to create first doctor (user ID 2)
3. POST to create second doctor (user ID 3)
4. Verify both appear in GET /api/doctors

### Scenario 2: Update Doctor Fee
1. Create a doctor with fee = 500
2. PUT to update fee to 600
3. GET to verify fee is updated
4. Verify old appointments use original fee

### Scenario 3: Availability Filtering
1. Create 3 doctors
2. Mark 1 as unavailable (isAvailable = false)
3. GET /api/doctors/available should return 2 doctors

### Scenario 4: License Uniqueness
1. Try to create second doctor with same license number
2. Expect 409 Conflict error

---

## 📚 Related Documentation

- See **DOCTOR_IMPLEMENTATION.md** for complete technical details
- See **README.md** for general system overview
- Check Swagger UI at `/swagger` for interactive API testing

---

## 🎯 Common Tasks

### Create Doctor from Scratch

1. First create User with Doctor role
2. Then create Doctor profile for that User

### Search Doctor by Name
- Use User endpoints to search users
- Filter results by Role = "Doctor"
- Then optionally get their Doctor profile

### Get Doctor's Appointments
- Use Doctor profile to get UserId
- Query appointments where DoctorId matches

---

## 💡 Tips

- Always include the JWT token in Authorization header for all requests
- License number format must be uppercase with hyphens
- Consultation fee will be used for automatic billing
- Mark doctors as unavailable during leaves/vacations
- Use specialization for patient-facing search/filter
- Clinic location helps distinguish multiple branch doctors

---

## 🔗 API Endpoint Summary

```
GET    /api/doctors                      # All doctors
GET    /api/doctors/{id}                 # By ID
GET    /api/doctors/user/{userId}        # By User ID
GET    /api/doctors/license/{number}     # By License Number
GET    /api/doctors/specialization/{spec}# By Specialization
GET    /api/doctors/available            # Available only
POST   /api/doctors                      # Create
PUT    /api/doctors/{id}                 # Update
DELETE /api/doctors/{id}                 # Delete
```

---

**Ready to use!** 🚀
