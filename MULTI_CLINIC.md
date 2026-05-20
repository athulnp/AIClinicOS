# Multi-Clinic (Multi-Tenant) Architecture

AI Clinic OS supports onboarding **multiple dental clinics** on one platform. Each clinic's data is isolated by `ClinicId`.

## Roles

| Role | Scope | Login |
|------|-------|-------|
| **SuperAdmin** | Platform — create/manage clinics | `username` + `password` (no `clinicCode`) |
| **Admin** | One clinic — users, settings | `clinicCode` + `username` + `password` |
| **Doctor** | One clinic — clinical work | Same as Admin |
| **Receptionist** | One clinic — front desk | Same as Admin |

## Tenant isolation

- JWT includes `clinic_id` and `clinic_code` claims for clinic staff.
- EF Core **global query filters** scope Patients, Appointments, Billing, Doctors, Reminders, and Users to the current clinic.
- Unique fields (username, email, patient code, invoice number, license number) are unique **per clinic**, not globally.

## SuperAdmin clinic context

For clinic-scoped APIs (users, patients, etc.), SuperAdmin must send:

```
X-Clinic-Id: <clinic numeric id>
```

Example: after `GET /api/clinics`, use the clinic `id` in the header when calling `GET /api/users`.

## Onboarding a new clinic

1. **SuperAdmin** `POST /api/clinics` with `code`, `name`, contact fields.
2. **SuperAdmin** `POST /api/users` with `X-Clinic-Id` and body including `role: Admin` (or use clinic Admin after step 3).
3. Clinic **Admin** logs in with `clinicCode` and creates doctors/receptionists via `POST /api/users`.
4. **Admin** `POST /api/doctors` to attach doctor profiles to doctor users.

## Database migrations

Schema is managed with **EF Core migrations** (`ClinicOS.Infrastructure/Migrations`). On startup the API runs `MigrateAsync()` then seeds demo data.

```bash
dotnet ef database update --project ClinicOS.Infrastructure --startup-project ClinicOS.API
```

For a clean dev database:

```sql
DROP DATABASE ClinicOSDb;
```

Then restart the API.

## Passwords

Passwords are stored with **BCrypt** (work factor 12). Existing SHA-256 hashes from older databases are verified on login and **re-hashed to BCrypt** automatically.

## Demo seed

| Clinic code | `demo-dental` |
| SuperAdmin | `superadmin` / `SuperAdmin@123` |
| Clinic Admin | `admin` / `Admin@123` (with `clinicCode: demo-dental`) |
| Doctor | `doctor1` / `Doctor@123` |
| Receptionist | `reception1` / `Reception@123` |

Login example (clinic staff) — use **string** `clinicCode` or numeric **`clinicId`**, not both:

```json
POST /api/auth/login
{
  "clinicCode": "demo-dental",
  "username": "admin",
  "password": "Admin@123"
}
```

Or by clinic id:

```json
{
  "clinicId": 1,
  "username": "admin",
  "password": "Admin@123"
}
```
