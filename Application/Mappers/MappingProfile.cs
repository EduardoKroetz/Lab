using AutoMapper;
using Lab.Api.Application.DTOs.Appointments;
using Lab.Api.Application.DTOs.Customers;
using Lab.Api.Application.DTOs.Offerings;
using Lab.Api.Application.DTOs.Tenants;
using Lab.Api.Application.DTOs.Users;
using Lab.Api.Domain.Entities;

namespace Lab.Api.Application.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Appointment, GetAppointmentDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.OfferingName, opt => opt.MapFrom(src => src.Offering == null ? null : src.Offering.Name))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByUser == null ? null : src.CreatedByUser.UserName));

        CreateMap<Customer, GetCustomerDto>();

        CreateMap<Offering, GetOfferingDto>();

        CreateMap<ApplicationUser, GetCurrentUserDto>();

        CreateMap<Tenant, GetTenantDto>();
    }
}
