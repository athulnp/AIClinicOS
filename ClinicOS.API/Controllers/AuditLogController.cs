using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet("recent")]
    public async Task<ActionResult<List<AuditLogDto>>> GetRecentActivities([FromQuery] int count = 10)
    {
        var clinicId = int.Parse(User.FindFirst("clinic_id")?.Value ?? "0");
        if (clinicId == 0)
            return BadRequest("Clinic ID not found in token");

        var activities = await _auditLogService.GetRecentActivitiesAsync(clinicId, count);
        var dtos = activities.Select(a => new AuditLogDto
        {
            Id = a.Id,
            ClinicId = a.ClinicId,
            UserId = a.UserId,
            UserName = a.UserName,
            Action = a.Action,
            EntityType = a.EntityType,
            EntityId = a.EntityId,
            EntityName = a.EntityName,
            Description = a.Description,
            Timestamp = a.Timestamp,
            TimeAgo = GetTimeAgo(a.Timestamp)
        }).ToList();

        return Ok(dtos);
    }

    private string GetTimeAgo(DateTime timestamp)
    {
        var span = DateTime.UtcNow - timestamp;
        if (span.TotalMinutes < 1)
            return "Just now";
        if (span.TotalMinutes < 60)
            return $"{(int)span.TotalMinutes} minute(s) ago";
        if (span.TotalHours < 24)
            return $"{(int)span.TotalHours} hour(s) ago";
        if (span.TotalDays < 7)
            return $"{(int)span.TotalDays} day(s) ago";
        return timestamp.ToString("MMM dd, yyyy");
    }
}
