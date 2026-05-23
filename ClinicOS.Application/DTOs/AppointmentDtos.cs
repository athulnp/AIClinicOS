using ClinicOS.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicOS.Application.DTOs;

/// <summary>
/// DTO for creating a new appointment
/// </summary>
public class CreateAppointmentDto
{
    [Required(ErrorMessage = "Patient ID is required")]
    public int PatientId { get; set; }

    [Required(ErrorMessage = "Doctor ID is required")]
    public int DoctorId { get; set; }

    [Required(ErrorMessage = "Appointment date is required")]
    public DateTime AppointmentDate { get; set; }

    [Required(ErrorMessage = "Start time is required")]
    public TimeSpan StartTime { get; set; }

    [Required(ErrorMessage = "End time is required")]
    public TimeSpan EndTime { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }

    // ClinicId is optional for regular staff (set from context), but required for super admins
    public int? ClinicId { get; set; }
}

/// <summary>
/// DTO for rescheduling an appointment
/// </summary>
public class RescheduleAppointmentDto
{
    [Required(ErrorMessage = "New appointment date is required")]
    public DateTime NewAppointmentDate { get; set; }

    [Required(ErrorMessage = "New start time is required")]
    public TimeSpan NewStartTime { get; set; }

    [Required(ErrorMessage = "New end time is required")]
    public TimeSpan NewEndTime { get; set; }
}

/// <summary>
/// DTO for updating appointment status
/// </summary>
public class UpdateAppointmentStatusDto
{
    [Required(ErrorMessage = "Status is required")]
    public AppointmentStatus Status { get; set; }

    [MaxLength(2000)]
    public string? Notes { get; set; }
}

/// <summary>
/// DTO for appointment response
/// </summary>
public class AppointmentDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO for doctor's daily schedule
/// </summary>
public class DoctorScheduleDto
{
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public List<AppointmentDto> Appointments { get; set; } = new();
}
