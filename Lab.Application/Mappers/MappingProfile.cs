using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.Appointments;
using Lab.Application.DTOs.Customers;
using Lab.Application.DTOs.Offerings;
using Lab.Application.DTOs.Tenants;
using Lab.Application.DTOs.Users;
using Lab.Domain.Entities;

namespace Lab.Application.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Appointment, GetAppointmentResponse>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.OfferingName, opt => opt.MapFrom(src => src.Offering == null ? null : src.Offering.Name));

        CreateMap<Customer, GetCustomerResponse>();
        CreateMap<Offering, GetOfferingResponse>();
        CreateMap<Tenant, GetTenantResponse>();

        CreateMap<IUser, GetCurrentUserResponse>();
    }
}
