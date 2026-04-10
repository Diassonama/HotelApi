using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Reserva.Queries
{
    public class GetAllReservasQuery : IRequest<IEnumerable<Domain.Entities.Reserva>>
    {
    }

    public class GetAllReservasQueryHandler : IRequestHandler<GetAllReservasQuery, IEnumerable<Domain.Entities.Reserva>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public GetAllReservasQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Domain.Entities.Reserva>> Handle(GetAllReservasQuery request, CancellationToken cancellationToken)
        {
            var reservas = await _unitOfWork.Reservas.ObterTodasReservaComApartamentosAsync();
            return reservas.Where(r => r.IsActive);
        }
    }
}
