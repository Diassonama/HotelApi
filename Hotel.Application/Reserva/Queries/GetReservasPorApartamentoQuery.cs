using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Reserva.Queries
{
    public class GetReservasPorApartamentoQuery : IRequest<BaseCommandResponse>
    {
        public int ApartamentoId { get; set; }
    }

    public class GetReservasPorApartamentoQueryHandler : IRequestHandler<GetReservasPorApartamentoQuery, BaseCommandResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApartamentoReservadoRepository _apartamentoReservadoRepository;

        public GetReservasPorApartamentoQueryHandler(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IApartamentoReservadoRepository apartamentoReservadoRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _apartamentoReservadoRepository = apartamentoReservadoRepository;
        }

        public async Task<BaseCommandResponse> Handle(GetReservasPorApartamentoQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();

            try
            {
                var reservasPorApartamento = await _apartamentoReservadoRepository.ObterReservasPorApartamentoAsync(request.ApartamentoId);

                response.Success = true;
                response.Message = "Reservas do apartamento encontradas com sucesso";
                response.Data = reservasPorApartamento;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Erro interno do servidor";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
