using AutoMapper;
using Lab.Application.Common.Interfaces;
using Lab.Application.DTOs.Assets;
using Lab.Application.DTOs.Controls;
using Lab.Application.DTOs.IncidentImpacts;
using Lab.Application.DTOs.Incidents;
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

        CreateMap<Control, GetControlResponse>();

        CreateMap<Incident, GetIncidentDetailResponse>().ForMember(dest => dest.Impacts, opt => opt.MapFrom(src => src.IncidentImpacts));
        CreateMap<Incident, GetIncidentListResponse>();

        CreateMap<IncidentImpact, GetIncidentImpactResponse>().ForMember(dest => dest.IncidentDescription, opt => opt.MapFrom(src => src.Incident.Description));

        CreateMap<Risk, GetRiskListResponse>()
            .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Asset.Name))
            .ForMember(dest => dest.ThreatName, opt => opt.MapFrom(src => src.Threat.Name))
            .ForMember(dest => dest.VulnerabilityName, opt => opt.MapFrom(src => src.Vulnerability.Name))
            .ForMember(dest => dest.RawScore, opt => opt.MapFrom(src => src.RawScore))
            .ForMember(dest => dest.ResidualScore, opt => opt.MapFrom(src => Math.Round(src.ResidualScore, 2)))
            .ForMember(dest => dest.EffectivenessOnProbability, opt => opt.MapFrom(src => Math.Round(src.EffectivenessOnProbability, 2)))
            .ForMember(dest => dest.EffectivenessOnImpact, opt => opt.MapFrom(src => Math.Round(src.EffectivenessOnImpact, 2)));

        CreateMap<Risk, GetRiskDetailResponse>()
             .ForMember(dest => dest.AssetName, opt => opt.MapFrom(src => src.Asset.Name))
             .ForMember(dest => dest.ThreatName, opt => opt.MapFrom(src => src.Threat.Name))
             .ForMember(dest => dest.VulnerabilityName, opt => opt.MapFrom(src => src.Vulnerability.Name))
             .ForMember(dest => dest.Controls, opt => opt.MapFrom(src => src.RiskControls))
             .ForMember(dest => dest.RawScore, opt => opt.MapFrom(src => src.RawScore))
             .ForMember(dest => dest.ResidualScore, opt => opt.MapFrom(src => Math.Round(src.ResidualScore, 2)))
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
