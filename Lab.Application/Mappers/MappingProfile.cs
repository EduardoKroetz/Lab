using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.Appointments;
using Lab.Application.DTOs.Assets;
using Lab.Application.DTOs.Controls;
using Lab.Application.DTOs.Customers;
using Lab.Application.DTOs.IncidentImpacts;
using Lab.Application.DTOs.Incidents;
using Lab.Application.DTOs.Offerings;
using Lab.Application.DTOs.RiskControls;
using Lab.Application.DTOs.Risks;
using Lab.Application.DTOs.Tenants;
using Lab.Application.DTOs.Threats;
using Lab.Application.DTOs.Users;
using Lab.Application.DTOs.Vulnerabilities;
using Lab.Domain.Entities;

namespace Lab.Application.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Asset, GetAssetResponse>();
        CreateMap<Appointment, GetAppointmentResponse>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
            .ForMember(dest => dest.OfferingName, opt => opt.MapFrom(src => src.Offering == null ? null : src.Offering.Name));

        CreateMap<Control, GetControlResponse>();
        CreateMap<Customer, GetCustomerResponse>();
        CreateMap<Incident, GetIncidentResponse>();
        CreateMap<IncidentImpact, GetIncidentImpactResponse>()
            .ForMember(dest => dest.IncidentDescription, opt => opt.MapFrom(src => src.Incident.Description));
        CreateMap<Offering, GetOfferingResponse>();

        CreateMap<Risk, GetRiskListResponse>()
            .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Asset.Name))
            .ForMember(dest => dest.ThreatName, opt => opt.MapFrom(src => src.Threat.Name))
            .ForMember(dest => dest.VulnerabilityName, opt => opt.MapFrom(src => src.Vulnerability.Name))
            .ForMember(dest => dest.Score, opt => opt.MapFrom(src => Math.Round(src.Score, 2)))
            .ForMember(dest => dest.EffectivenessOnProbability, opt => opt.MapFrom(src => Math.Round(src.EffectivenessOnProbability, 2)))
            .ForMember(dest => dest.EffectivenessOnImpact, opt => opt.MapFrom(src => Math.Round(src.EffectivenessOnImpact, 2)));

        CreateMap<Risk, GetRiskDetailResponse>()
             .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Asset.Name))
             .ForMember(dest => dest.ThreatName, opt => opt.MapFrom(src => src.Threat.Name))
             .ForMember(dest => dest.VulnerabilityName, opt => opt.MapFrom(src => src.Vulnerability.Name))
             .ForMember(dest => dest.Controls, opt => opt.MapFrom(src => src.RiskControls))
             .ForMember(dest => dest.Score, opt => opt.MapFrom(src => Math.Round(src.Score, 2)))
             .ForMember(dest => dest.EffectivenessOnProbability, opt => opt.MapFrom(src => Math.Round(src.EffectivenessOnProbability, 2)))
             .ForMember(dest => dest.EffectivenessOnImpact, opt => opt.MapFrom(src => Math.Round(src.EffectivenessOnImpact, 2)));

        CreateMap<RiskControl, GetRiskControlResponse>()
            .ForMember(dest => dest.ControlName, opt => opt.MapFrom(src => src.Control.Name));

        CreateMap<Tenant, GetTenantResponse>();
        CreateMap<Threat, GetThreatResponse>();
        CreateMap<Vulnerability, GetVulnerabilityResponse>();

        CreateMap<IUser, GetCurrentUserResponse>();
    }
}
