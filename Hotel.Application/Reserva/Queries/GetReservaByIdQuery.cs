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
    public class GetReservaByIdQuery : IRequest<BaseCommandResponse>
    {
        public int Id { get; set; }
    }

    public class GetReservaByIdQueryHandler : IRequestHandler<GetReservaByIdQuery, BaseCommandResponse>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IReservaRepository _reservaRepository;

        public GetReservaByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IReservaRepository reservaRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _reservaRepository = reservaRepository;
        }

        public async Task<BaseCommandResponse> Handle(GetReservaByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();

            try
            {
                var reserva = await _reservaRepository.ObterReservaComApartamentosAsync(request.Id);
                
                if (reserva == null)
                {
                    response.Success = false;
                    response.Message = "Reserva não encontrada";
                    return response;
                }

                response.Success = true;
                response.Message = "Reserva encontrada com sucesso";
                response.Data = reserva;
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
