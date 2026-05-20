using AutoMapper;
using ClinicOS.Application.DTOs;
using ClinicOS.Domain.Entities;

namespace ClinicOS.Application.Mapping;

/// <summary>
/// AutoMapper profile for entity-DTO mappings
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Patient mappings
        CreateMap<CreatePatientDto, Patient>();
        CreateMap<UpdatePatientDto, Patient>();
        CreateMap<Patient, PatientDto>();

        // Appointment mappings
        CreateMap<CreateAppointmentDto, Appointment>();
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : ""))
            .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName : ""));

        // Billing mappings
        CreateMap<CreateBillingDto, Billing>();
        CreateMap<Billing, BillingDto>()
            .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.FullName : ""));

        // User mappings
        CreateMap<CreateUserDto, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()); // Password hash is set in service
        CreateMap<UpdateUserDto, User>();
        CreateMap<User, UserDto>();

        // Reminder mappings
        CreateMap<Reminder, ReminderDto>();

        // Doctor mappings
        CreateMap<CreateDoctorDto, Doctor>();
        CreateMap<UpdateDoctorDto, Doctor>();
        CreateMap<Doctor, DoctorResponseDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : ""))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : ""))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : ""));
    }
}
