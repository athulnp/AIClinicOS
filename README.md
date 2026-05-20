# Clinic OS Lite - Dental Clinic Management System

A production-ready ASP.NET Core 8 backend for dental clinic management, built with clean architecture and enterprise-grade practices.

## 🏗️ Architecture

This project follows **Clean Architecture** principles with separation of concerns across four layers:

### Project Structure

```
ClinicOS/
├── ClinicOS.API/              # Presentation Layer (Controllers, Middleware)
├── ClinicOS.Application/      # Application Layer (DTOs, Services, Validators)
├── ClinicOS.Domain/           # Domain Layer (Entities, Enums, Interfaces)
├── ClinicOS.Infrastructure/   # Infrastructure Layer (DbContext, Repositories)
├── Dockerfile
├── docker-compose.yml
└── README.md
```

### Architecture Layers

1. **Domain Layer** (`ClinicOS.Domain`)
   - Core business entities
   - Enums and value objects
   - Base classes (BaseEntity, AuditableEntity, SoftDeleteEntity)
   - No external dependencies

2. **Application Layer** (`ClinicOS.Application`)
   - DTOs for data transfer
   - Service interfaces and implementations
   - FluentValidation validators
   - AutoMapper profiles
   - Business logic

3. **Infrastructure Layer** (`ClinicOS.Infrastructure`)
   - Entity Framework Core DbContext
   - Repository implementations
   - Database configuration
   - External service integrations

4. **API Layer** (`ClinicOS.API`)
   - RESTful API controllers
   - JWT Authentication
   - Swagger/OpenAPI documentation
   - Serilog logging
   - Global exception handling
   - Background services

## 🚀 Features

### 1. Authentication & User Management
- JWT-based authentication
- Role-based authorization (Admin, Doctor, Receptionist)
- User CRUD operations
- Refresh token support
- Password hashing

### 2. Patient Records
- Patient CRUD with soft delete
- Search by name or phone number
- Pagination support
- Patient code generation
- Medical history and allergies tracking

### 3. Appointments
- Create, reschedule, cancel appointments
- Status tracking (Scheduled, Completed, Cancelled, NoShow)
- Overlap prevention for doctor schedules
- Doctor daily schedule view
- Patient appointment history

### 4. Billing & Payments
- Invoice generation
- Payment tracking
- Outstanding balance reports
- Multiple payment methods (Cash, UPI, Card, BankTransfer)
- Payment status tracking (Pending, Partial, Paid)

### 5. Reminder System
- Background service for automated reminders
- Appointment reminders (24 hours before)
- Follow-up reminders (3 days after)
- Reminder logs and status tracking
- Architecture ready for WhatsApp/SMS integration

## 🛠️ Tech Stack

- **.NET 8** / ASP.NET Core Web API
- **Entity Framework Core** 10.0
- **SQL Server** 2022
- **JWT Authentication** (Microsoft.AspNetCore.Authentication.JwtBearer)
- **Swagger/OpenAPI** (Swashbuckle.AspNetCore)
- **Serilog** for structured logging
- **FluentValidation** for request validation
- **AutoMapper** for object mapping
- **Docker** & Docker Compose
- **Clean Architecture** pattern
- **Repository + Service** pattern

## 📋 Prerequisites

- .NET 10 SDK
- SQL Server 2022 (or use Docker)
- Docker Desktop (optional, for containerized deployment)

## 🔧 Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd ClinicOS
```

### 2. Configure Connection String

Update `appsettings.json` in `ClinicOS.API`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ClinicOSDb;User Id=sa;Password=YourStrong@Password;TrustServerCertificate=True"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "ClinicOS",
    "Audience": "ClinicOSUsers"
  }
}
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Run Database Migrations

```bash
cd ClinicOS.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../ClinicOS.API
dotnet ef database update --startup-project ../ClinicOS.API
```

### 5. Run the Application

```bash
cd ClinicOS.API
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001` (root URL)

### 6. Default Seed Data

The application automatically seeds the following users:

| Username | Password | Role | Email |
|----------|----------|------|-------|
| admin | Admin@123 | Admin | admin@clinicos.com |
| doctor1 | Doctor@123 | Doctor | doctor1@clinicos.com |
| reception1 | Reception@123 | Receptionist | reception1@clinicos.com |

## 🐳 Docker Deployment

### Using Docker Compose (Recommended)

```bash
docker-compose up -d
```

This will start:
- SQL Server on port 1433
- API on ports 5000 (HTTP) and 5001 (HTTPS)

### Using Docker Build

```bash
docker build -t clinicos-api .
docker run -p 5000:80 -e ConnectionStrings__DefaultConnection="Server=host.docker.internal;Database=ClinicOSDb;User Id=sa;Password=YourStrong@Password;TrustServerCertificate=True" clinicos-api
```

## 📚 API Documentation

### Swagger UI

Access the interactive API documentation at:
- Development: `https://localhost:5001`
- Production: `http://localhost:5000/swagger`

### Authentication

Most endpoints require JWT authentication. Include the token in the Authorization header:

```
Authorization: Bearer <your-jwt-token>
```

### Key Endpoints

#### Authentication
- `POST /api/auth/login` - Login and get JWT token
- `POST /api/auth/refresh` - Refresh JWT token
- `POST /api/auth/logout` - Logout
- `GET /api/auth/me` - Get current user

#### Patients
- `GET /api/patients` - Get all patients (paginated)
- `GET /api/patients/search` - Search patients
- `GET /api/patients/{id}` - Get patient by ID
- `POST /api/patients` - Create new patient
- `PUT /api/patients/{id}` - Update patient
- `DELETE /api/patients/{id}` - Delete patient (soft delete)

#### Appointments
- `GET /api/appointments` - Get all appointments (paginated)
- `GET /api/appointments/{id}` - Get appointment by ID
- `GET /api/appointments/patient/{patientId}` - Get patient appointments
- `GET /api/appointments/doctor/{doctorId}/schedule` - Get doctor schedule
- `POST /api/appointments` - Create appointment
- `PUT /api/appointments/{id}/reschedule` - Reschedule appointment
- `PUT /api/appointments/{id}/status` - Update appointment status
- `PUT /api/appointments/{id}/cancel` - Cancel appointment

#### Billing
- `GET /api/billing` - Get all billings (paginated)
- `GET /api/billing/{id}` - Get billing by ID
- `GET /api/billing/invoice/{invoiceNumber}` - Get billing by invoice number
- `GET /api/billing/patient/{patientId}` - Get patient billing history
- `GET /api/billing/outstanding` - Get outstanding balance report
- `POST /api/billing` - Create billing record
- `POST /api/billing/{id}/payment` - Record payment

#### Reminders
- `GET /api/reminders/logs` - Get reminder logs
- `POST /api/reminders/send-appointment-reminders` - Send appointment reminders (Admin)
- `POST /api/reminders/send-follow-up-reminders` - Send follow-up reminders (Admin)

## 🔐 Security Features

- **JWT Authentication** with configurable expiration
- **Role-based Authorization** (Admin, Doctor, Receptionist)
- **Password Hashing** using SHA-256 (upgrade to BCrypt in production)
- **CORS Configuration** for cross-origin requests
- **API Key Middleware** (optional, for future use)
- **Global Exception Handling** to prevent information leakage

## 📊 Database Schema

### Tables

1. **Users** - System users with roles
2. **Patients** - Patient records with medical history
3. **Appointments** - Scheduled appointments
4. **Billings** - Invoice and payment records
5. **Reminders** - Reminder logs and status

### Key Features

- **Soft Delete** support for Patients
- **Audit Fields** (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
- **Indexes** on frequently queried fields
- **Foreign Key Constraints** for data integrity
- **Unique Constraints** on PatientCode, Username, Email, InvoiceNumber

## 🧪 Testing

The project structure is ready for unit testing. Create test projects:

```bash
dotnet new xunit -n ClinicOS.Application.Tests
dotnet new xunit -n ClinicOS.Infrastructure.Tests
dotnet new xunit -n ClinicOS.API.Tests
```

## 🔮 Future Enhancements

The architecture is designed to support future features:

- **Multi-tenant SaaS** support
- **AI Voice Notes** integration
- **WhatsApp/SMS** reminders
- **Multi-clinic** management
- **Doctor Notes** module
- **Dental Image Uploads** (X-rays, photos)
- **Subscription Billing**
- **Analytics Dashboard**
- **Mobile App APIs**

## 📝 Coding Standards

- **SOLID Principles** throughout
- **Async/Await** for all I/O operations
- **Clean Naming Conventions** (PascalCase for public members)
- **Dependency Injection** for loose coupling
- **Interface-based** programming
- **Repository Pattern** for data access
- **Service Layer** for business logic
- **DTO Pattern** for data transfer
- **FluentValidation** for request validation

## 🤝 Contributing

1. Follow the existing code style
2. Add unit tests for new features
3. Update API documentation
4. Ensure all tests pass before PR

## 📄 License

This project is proprietary software. All rights reserved.

## 👥 Support

For support, contact the development team at support@clinicos.com

---

**Built with ❤️ using ASP.NET Core 8 and Clean Architecture**
