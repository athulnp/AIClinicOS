using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;

namespace ClinicOS.Application.Interfaces;

public interface IClinicService
{
    Task<ApiResponse<ClinicDto>> CreateClinicAsync(CreateClinicDto dto, string createdBy);
    Task<ApiResponse<ClinicDto>> UpdateClinicAsync(int id, UpdateClinicDto dto, string updatedBy);
    Task<ApiResponse> DeleteClinicAsync(int id, string deletedBy);
    Task<ApiResponse<ClinicDto>> GetClinicByIdAsync(int id);
    Task<ApiResponse<ClinicDto>> GetClinicByCodeAsync(string code);
    Task<PagedResponse<ClinicDto>> GetAllClinicsAsync(PaginationRequest pagination, bool? isActive = null);
}
