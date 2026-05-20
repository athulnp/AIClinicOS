using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOS.API.Controllers;

/// <summary>
/// Reminder management controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RemindersController : ControllerBase
{
    private readonly IReminderService _reminderService;

    public RemindersController(IReminderService reminderService)
    {
        _reminderService = reminderService;
    }

    /// <summary>
    /// Get reminder logs
    /// </summary>
    [HttpGet("logs")]
    public async Task<ActionResult<ReminderLogDto>> GetReminderLogs(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var result = await _reminderService.GetReminderLogsAsync(fromDate, toDate);
        if (!result.Success)
        {
            return BadRequest(result);
        }
        return Ok(result.Data);
    }

    /// <summary>
    /// Manually trigger appointment reminders (Admin only)
    /// </summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost("send-appointment-reminders")]
    public async Task<ActionResult> SendAppointmentReminders()
    {
        await _reminderService.SendAppointmentRemindersAsync();
        return Ok(new { message = "Appointment reminders sent successfully" });
    }

    /// <summary>
    /// Manually trigger follow-up reminders (Admin only)
    /// </summary>
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost("send-follow-up-reminders")]
    public async Task<ActionResult> SendFollowUpReminders()
    {
        await _reminderService.SendFollowUpRemindersAsync();
        return Ok(new { message = "Follow-up reminders sent successfully" });
    }
}
