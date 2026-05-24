using ClinicOS.Application.Interfaces;
using ClinicOS.Domain.Entities;
using ClinicOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClinicOS.Infrastructure.Repositories;

public class AppointmentNoteRepository : IAppointmentNoteRepository
{
    private readonly AppDbContext _context;

    public AppointmentNoteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppointmentNote>> GetByAppointmentIdAsync(int appointmentId)
    {
        return await _context.AppointmentNotes
            .Where(an => an.AppointmentId == appointmentId)
            .OrderByDescending(an => an.CreatedAt)
            .ToListAsync();
    }

    public async Task<AppointmentNote?> GetByIdAsync(int id)
    {
        return await _context.AppointmentNotes.FindAsync(id);
    }

    public async Task<AppointmentNote> AddAsync(AppointmentNote note)
    {
        _context.AppointmentNotes.Add(note);
        await _context.SaveChangesAsync();
        return note;
    }

    public void Update(AppointmentNote note)
    {
        _context.AppointmentNotes.Update(note);
    }

    public void Delete(AppointmentNote note)
    {
        _context.AppointmentNotes.Remove(note);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}
