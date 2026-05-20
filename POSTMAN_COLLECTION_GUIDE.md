# 📮 Postman Collection Update - Doctor Endpoints Added

## ✅ What Was Updated

The `PostmanCollection.json` file has been updated to include **9 new Doctor API endpoints**.

---

## 🔌 New Endpoints Added

### 1. **Get All Doctors**
```
GET /api/doctors
```
- **Authorization**: Required (Any authenticated user)
- **Description**: Retrieve list of all doctors
- **Response**: Array of doctor objects

### 2. **Get Doctor by ID**
```
GET /api/doctors/1
```
- **Authorization**: Required
- **Description**: Get specific doctor by ID
- **Response**: Single doctor object
- **Note**: Replace `1` with actual doctor ID

### 3. **Get Doctor by User ID**
```
GET /api/doctors/user/2
```
- **Authorization**: Required
- **Description**: Get doctor profile for a specific user
- **Response**: Single doctor object
- **Note**: Replace `2` with actual user ID

### 4. **Get Doctor by License Number**
```
GET /api/doctors/license/DEN-2024-001
```
- **Authorization**: Required
- **Description**: Search doctor by license number (unique)
- **Response**: Single doctor object
- **Note**: Replace `DEN-2024-001` with actual license number

### 5. **Get Doctors by Specialization**
```
GET /api/doctors/specialization/Orthodontist
```
- **Authorization**: Required
- **Description**: Filter doctors by specialization
- **Response**: Array of doctor objects with matching specialization
- **Note**: Replace `Orthodontist` with desired specialization

### 6. **Get Available Doctors**
```
GET /api/doctors/available
```
- **Authorization**: Required
- **Description**: Get only available doctors (isAvailable=true and user is active)
- **Response**: Array of available doctor objects
- **Use Case**: For appointment booking interface

### 7. **Create Doctor (Admin Only)**
```
POST /api/doctors
```
- **Authorization**: Required (Admin role)
- **Description**: Create new doctor profile
- **Request Body**:
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
- **Response**: Created doctor object (201 Created)

### 8. **Update Doctor**
```
PUT /api/doctors/1
```
- **Authorization**: Required (Admin or Doctor own profile)
- **Description**: Update doctor details (partial update allowed)
- **Request Body**: Same structure as Create (all fields optional)
- **Response**: Updated doctor object
- **Note**: Only provide fields you want to update

### 9. **Delete Doctor (Admin Only)**
```
DELETE /api/doctors/1
```
- **Authorization**: Required (Admin role)
- **Description**: Delete doctor profile
- **Response**: 204 No Content
- **Note**: Replace `1` with actual doctor ID

---

## 📋 Collection Structure

The updated Postman collection now has the following structure:

```
Clinic OS Lite API
├── Authentication (4 endpoints)
├── Patients (6 endpoints)
├── Appointments (6 endpoints)
├── Billing (4 endpoints)
├── Reminders (2 endpoints)
└── Doctors (9 endpoints) ← NEW
    ├── Get All Doctors
    ├── Get Doctor by ID
    ├── Get Doctor by User ID
    ├── Get Doctor by License Number
    ├── Get Doctors by Specialization
    ├── Get Available Doctors
    ├── Create Doctor (Admin)
    ├── Update Doctor
    └── Delete Doctor (Admin)
```

---

## 🚀 How to Import & Use

### Step 1: Import Collection in Postman

**Option A: Direct Import**
1. Open Postman
2. Click **"Import"** button
3. Select the updated `PostmanCollection.json` file
4. Click **"Import"**

**Option B: Use from Repository**
1. Clone repository
2. In Postman, click **"Collections"** → **"Import"**
3. Browse to `PostmanCollection.json`

### Step 2: Set Variables

The collection uses variables:
- `{{baseUrl}}` - Default: `https://localhost:5001/api`
- `{{token}}` - Your JWT token (set after login)

To update:
1. Right-click collection name
2. Click **"Edit"**
3. Go to **"Variables"** tab
4. Update `baseUrl` if your server uses different port

### Step 3: Authenticate

1. Go to **Authentication** → **Login**
2. Use credentials:
   - Username: `admin`
   - Password: `Admin@123`
3. Click **"Send"**
4. Copy the `token` from response
5. In Postman:
   - Click collection name
   - Go to **"Variables"** tab
   - Paste token in `token` value
   - Click **"Save"**

### Step 4: Test Doctor Endpoints

Now all Doctor endpoints are ready to use!

---

## 📝 Example Workflows

### Workflow 1: Create and Test Doctor

1. **Login** (Authentication → Login)
2. **Create Doctor** (Doctors → Create Doctor)
   - Update `userId` if needed
   - Adjust other fields
   - Click Send
   - Note the returned doctor ID
3. **Get Doctor** (Doctors → Get Doctor by ID)
   - Use the ID from step 2
   - Click Send

### Workflow 2: Find Available Doctors for Appointments

1. **Login**
2. **Get Available Doctors** (Doctors → Get Available Doctors)
3. Use returned doctors for appointment booking

### Workflow 3: Search by Specialization

1. **Login**
2. **Get Doctors by Specialization** (Doctors → Get Doctors by Specialization)
   - Change specialization in URL (e.g., "Pediatric Dentist")
   - Click Send

---

## 🔐 Authorization Notes

| Endpoint | Public | Admin | Doctor | Receptionist |
|----------|--------|-------|--------|--------------|
| GET /api/doctors* | ✓ | ✓ | ✓ | ✓ |
| POST /api/doctors | ✗ | ✓ | ✗ | ✗ |
| PUT /api/doctors | ✗ | ✓ | ✓* | ✗ |
| DELETE /api/doctors | ✗ | ✓ | ✗ | ✗ |

*Doctor can only update own profile

---

## ✨ Key Features in Collection

✓ **Pre-built requests** - No need to manually create URLs
✓ **Sample data** - All requests include example JSON bodies
✓ **Authorization headers** - Bearer token automatically included
✓ **Variables** - Easy to switch between environments
✓ **Comments** - Clear endpoint descriptions
✓ **Testing ready** - Can be used for API testing workflows

---

## 🎯 Common Use Cases

### Use Case 1: Appointment Booking
```
1. GET /api/doctors/available
   → Get available doctors
2. GET /api/doctors/specialization/{{specialization}}
   → Filter by specialty
3. User selects a doctor
4. Use doctor's consultationFee in billing
```

### Use Case 2: Doctor Management (Admin)
```
1. GET /api/doctors
   → View all doctors
2. POST /api/doctors
   → Create new doctor
3. PUT /api/doctors/{{id}}
   → Update doctor info
4. DELETE /api/doctors/{{id}}
   → Remove doctor
```

### Use Case 3: Doctor Self-Service
```
1. GET /api/doctors/user/{{userId}}
   → Get own profile
2. PUT /api/doctors/{{id}}
   → Update own details
```

---

## 📊 Complete Endpoint Summary

| Method | Endpoint | Role | Purpose |
|--------|----------|------|---------|
| GET | /api/doctors | All | Get all doctors |
| GET | /api/doctors/{id} | All | Get by ID |
| GET | /api/doctors/user/{userId} | All | Get by User ID |
| GET | /api/doctors/license/{number} | All | Get by License |
| GET | /api/doctors/specialization/{spec} | All | Filter by specialty |
| GET | /api/doctors/available | All | Get available only |
| POST | /api/doctors | Admin | Create |
| PUT | /api/doctors/{id} | Admin/Doctor | Update |
| DELETE | /api/doctors/{id} | Admin | Delete |

---

## 🔄 Integration with Other Modules

### Appointments Integration
```
When creating appointment:
- GET /api/doctors/available
  → Use to show available doctors dropdown
- POST /api/appointments
  → Use selected doctorId
```

### Billing Integration
```
When creating billing:
- GET /api/doctors/{id}
  → Get doctor's consultationFee
- POST /api/billing
  → Use consultationFee in totalAmount
```

---

## ✅ Testing Checklist

- [ ] Import PostmanCollection.json
- [ ] Set {{baseUrl}} variable correctly
- [ ] Login and get JWT token
- [ ] Set {{token}} variable
- [ ] Test GET /api/doctors (should return empty array)
- [ ] Test POST /api/doctors (create sample doctor)
- [ ] Test GET /api/doctors by ID
- [ ] Test GET /api/doctors/available
- [ ] Test GET by specialization
- [ ] Test PUT /api/doctors (update)
- [ ] Test DELETE /api/doctors

---

## 💡 Tips

1. **Save responses** - Right-click response → Save as example
2. **Create test cases** - Each request can have tests
3. **Use environments** - Create dev/staging/production environments
4. **Export results** - Generate reports from test runs
5. **Monitor API** - Use Postman's monitoring features

---

## 📧 Support

For API issues:
- Check logs: `c:\Athul\AIDentalTool\ClinicOS.API\logs\`
- Review endpoint documentation in Swagger UI
- See `DOCTOR_IMPLEMENTATION.md` for technical details

---

**Collection Updated**: 2026-05-20  
**Status**: ✅ Ready to Use  
**Total Endpoints**: 31 (including 9 new Doctor endpoints)
