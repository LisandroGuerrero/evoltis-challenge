using EvoltisChallenge.Models;
using EvoltisChallenge.DTOs;
using AutoMapper;

namespace EvoltisChallenge.Services.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UsuarioDto, Usuario>();
            CreateMap<UpdateUsuarioDto, Usuario>()
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.ID, opt => opt.Ignore());

            CreateMap<Usuario, UsuarioResponseDto>();

            CreateMap<DomicilioDto, Domicilio>()
                .ForMember(dest => dest.ID, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioID, opt => opt.Ignore())
                .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
                .ForMember(dest => dest.Usuario, opt => opt.Ignore());

            CreateMap<Domicilio, DomicilioResponseDto>();
        }
    }

}
