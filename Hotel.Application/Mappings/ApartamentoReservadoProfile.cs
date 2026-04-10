using AutoMapper;
using Hotel.Application.DTOs.ApartamentoReservado;
using Hotel.Domain.Entities;

namespace Hotel.Application.Mappings
{
    /// <summary>
    /// Perfil de mapeamento para ApartamentoReservado
    /// </summary>
    public class ApartamentoReservadoProfile : Profile
    {
        public ApartamentoReservadoProfile()
        {
            CreateMap<ApartamentosReservado, ApartamentoReservadoDto>()
                .ForMember(dest => dest.ApartamentoId, opt => opt.MapFrom(src => src.ApartamentosId))
                .ForMember(dest => dest.ReservaId, opt => opt.MapFrom(src => src.ReservasId))
                .ForMember(dest => dest.NumeroApartamento, opt => opt.Ignore())
                .ForMember(dest => dest.TipoApartamento, opt => opt.Ignore())
                .ForMember(dest => dest.NomeCliente, opt => opt.Ignore())
                .ForMember(dest => dest.EmailCliente, opt => opt.Ignore())
                .ForMember(dest => dest.NomeUsuario, opt => opt.Ignore());

            CreateMap<ApartamentoReservadoDto, ApartamentosReservado>()
                .ForMember(dest => dest.ApartamentosId, opt => opt.MapFrom(src => src.ApartamentoId))
                .ForMember(dest => dest.ReservasId, opt => opt.MapFrom(src => src.ReservaId))
                .ForMember(dest => dest.Apartamentos, opt => opt.Ignore())
                .ForMember(dest => dest.Reservas, opt => opt.Ignore())
                .ForMember(dest => dest.Clientes, opt => opt.Ignore())
                .ForMember(dest => dest.TipoHospedagens, opt => opt.Ignore())
                .ForMember(dest => dest.Utilizadores, opt => opt.Ignore());
        }
    }
}
