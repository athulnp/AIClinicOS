using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;

namespace ClinicOS.Application.Interfaces;

/// <summary>
/// Patient service interface
/// </summary>
public interface IPatientService
{
    Task<ApiResponse<PatientDto>> CreatePatientAsync(CreatePatientDto dto, string createdBy, int clinicId);
    Task<ApiResponse<PatientDto>> UpdatePatientAsync(int id, UpdatePatientDto dto, string updatedBy);
    Task<ApiResponse> DeletePatientAsync(int id, string deletedBy);
    Task<ApiResponse<PatientDto>> GetPatientByIdAsync(int id);
    Task<ApiResponse<PatientDto>> GetPatientByCodeAsync(string patientCode);
    Task<PagedResponse<PatientDto>> SearchPatientsAsync(string? searchTerm, PaginationRequest pagination);
    Task<PagedResponse<PatientDto>> GetAllPatientsAsync(PaginationRequest pagination, int? clinicId = null);
}
