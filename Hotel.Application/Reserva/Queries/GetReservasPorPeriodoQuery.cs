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
    public class GetReservasPorPeriodoQuery : IRequest<BaseCommandResponse>
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
    }

    public class GetReservasPorPeriodoQueryHandler : IRequestHandler<GetReservasPorPeriodoQuery, BaseCommandResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IApartamentoReservadoRepository _apartamentoReservadoRepository;

        public GetReservasPorPeriodoQueryHandler(
            IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IApartamentoReservadoRepository apartamentoReservadoRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _apartamentoReservadoRepository = apartamentoReservadoRepository;
        }

        public async Task<BaseCommandResponse> Handle(GetReservasPorPeriodoQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();

            try
            {
                var reservasPorPeriodo = await _apartamentoReservadoRepository.ObterReservasPorPeriodoAsync(
                    request.DataInicio, 
                    request.DataFim);

                response.Success = true;
                response.Message = "Reservas encontradas com sucesso";
                response.Data = reservasPorPeriodo;
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
