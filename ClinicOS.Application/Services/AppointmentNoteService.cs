using AutoMapper;
using ClinicOS.Application.Common;
using ClinicOS.Application.DTOs;
using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Services;

public class AppointmentNoteService : IAppointmentNoteService
{
    private readonly IAppointmentNoteRepository _noteRepository;
    private readonly IMapper _mapper;

    public AppointmentNoteService(IAppointmentNoteRepository noteRepository, IMapper mapper)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<AppointmentNoteDto>>> GetNotesByAppointmentIdAsync(int appointmentId)
    {
        var notes = await _noteRepository.GetByAppointmentIdAsync(appointmentId);
        var noteDtos = _mapper.Map<List<AppointmentNoteDto>>(notes);
        return ApiResponse<List<AppointmentNoteDto>>.SuccessResponse(noteDtos);
    }

    public async Task<ApiResponse<AppointmentNoteDto>> GetNoteByIdAsync(int id)
    {
        var note = await _noteRepository.GetByIdAsync(id);
        if (note == null)
        {
            return ApiResponse<AppointmentNoteDto>.ErrorResponse("Note not found");
        }

        var noteDto = _mapper.Map<AppointmentNoteDto>(note);
        return ApiResponse<AppointmentNoteDto>.SuccessResponse(noteDto);
    }

    public async Task<ApiResponse<AppointmentNoteDto>> CreateNoteAsync(int appointmentId, CreateAppointmentNoteDto dto, string createdBy)
    {
        var note = new AppointmentNote
        {
            AppointmentId = appointmentId,
            Content = dto.Content,
            NoteType = dto.NoteType,
            CreatedByUser = createdBy,
            CreatedBy = createdBy
        };

        var createdNote = await _noteRepository.AddAsync(note);
        var noteDto = _mapper.Map<AppointmentNoteDto>(createdNote);
        return ApiResponse<AppointmentNoteDto>.SuccessResponse(noteDto, "Note added successfully");
    }

    public async Task<ApiResponse<AppointmentNoteDto>> UpdateNoteAsync(int id, UpdateAppointmentNoteDto dto, string updatedBy)
    {
        var note = await _noteRepository.GetByIdAsync(id);
        if (note == null)
        {
            return ApiResponse<AppointmentNoteDto>.ErrorResponse("Note not found");
        }

        note.Content = dto.Content;
        note.UpdatedBy = updatedBy;
        _noteRepository.Update(note);
        await _noteRepository.SaveAsync();

        var noteDto = _mapper.Map<AppointmentNoteDto>(note);
        return ApiResponse<AppointmentNoteDto>.SuccessResponse(noteDto, "Note updated successfully");
    }

    public async Task<ApiResponse> DeleteNoteAsync(int id)
    {
        var note = await _noteRepository.GetByIdAsync(id);
        if (note == null)
        {
            return ApiResponse.ErrorResponse("Note not found");
        }

        _noteRepository.Delete(note);
        await _noteRepository.SaveAsync();

        return ApiResponse.SuccessResponse("Note deleted successfully");
    }
}
