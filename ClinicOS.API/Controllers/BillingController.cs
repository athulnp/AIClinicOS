using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicOS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BillingController : ControllerBase
{
    private readonly IBillingService _billingService;

    public BillingController(IBillingService billingService)
    {
        _billingService = billingService;
    }

    [Authorize(Policy = PermissionCodes.BillingRead)]
    [HttpGet]
    public async Task<ActionResult<PagedResponse<BillingDto>>> GetAllBillings([FromQuery] PaginationRequest pagination)
    {
        var result = await _billingService.GetAllBillingsAsync(pagination);
        return Ok(result);
    }

    [Authorize(Policy = PermissionCodes.BillingRead)]
    [HttpGet("{id}")]
    public async Task<ActionResult<BillingDto>> GetBilling(int id)
    {
        var result = await _billingService.GetBillingByIdAsync(id);
        if (!result.Success)
            return NotFound(result);
        return Ok(result.Data);
    }

    [Authorize(Policy = PermissionCodes.BillingRead)]
    [HttpGet("invoice/{invoiceNumber}")]
    public async Task<ActionResult<BillingDto>> GetBillingByInvoiceNumber(string invoiceNumber)
    {
        var result = await _billingService.GetBillingByInvoiceNumberAsync(invoiceNumber);
        if (!result.Success)
            return NotFound(result);
        return Ok(result.Data);
    }

    [Authorize(Policy = PermissionCodes.BillingRead)]
    [HttpGet("patient/{patientId}")]
    public async Task<ActionResult<PagedResponse<BillingDto>>> GetPatientBillingHistory(
        int patientId,
        [FromQuery] PaginationRequest pagination)
    {
        var result = await _billingService.GetPatientBillingHistoryAsync(patientId, pagination);
        return Ok(result);
    }

    [Authorize(Policy = PermissionCodes.BillingRead)]
    [HttpGet("outstanding")]
    public async Task<ActionResult<PagedResponse<OutstandingBalanceReportDto>>> GetOutstandingBalanceReport(
        [FromQuery] PaginationRequest pagination)
    {
        var result = await _billingService.GetOutstandingBalanceReportAsync(pagination);
        return Ok(result);
    }

    [Authorize(Policy = PermissionCodes.BillingWrite)]
    [HttpPost]
    public async Task<ActionResult<BillingDto>> CreateBilling([FromBody] CreateBillingDto dto)
    {
        var createdBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _billingService.CreateBillingAsync(dto, createdBy);
        if (!result.Success)
            return BadRequest(result);
        return CreatedAtAction(nameof(GetBilling), new { id = result.Data!.Id }, result.Data);
    }

    [Authorize(Policy = PermissionCodes.BillingWrite)]
    [HttpPost("{id}/payment")]
    public async Task<ActionResult<BillingDto>> RecordPayment(
        int id,
        [FromBody] RecordPaymentDto dto)
    {
        var updatedBy = User.FindFirst(ClaimTypes.Name)?.Value ?? "System";
        var result = await _billingService.RecordPaymentAsync(id, dto, updatedBy);
        if (!result.Success)
            return BadRequest(result);
        return Ok(result.Data);
    }
}
