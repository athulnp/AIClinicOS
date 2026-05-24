using System.ComponentModel.DataAnnotations;

namespace ClinicOS.Application.DTOs;

public class CreateAppointmentNoteDto
{
    [Required(ErrorMessage = "Content is required")]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? NoteType { get; set; }
}

public class UpdateAppointmentNoteDto
{
    [Required(ErrorMessage = "Content is required")]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}

public class AppointmentNoteDto
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? NoteType { get; set; }
    public string CreatedByUser { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
