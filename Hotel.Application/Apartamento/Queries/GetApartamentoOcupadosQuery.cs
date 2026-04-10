
using AutoMapper;
using Hotel.Application.Apartamento.Base;
using Hotel.Domain.Interface;
using Hotel.Domain.Entities;
//using Hotel.Infrastruture.Persistence.Context;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hotel.Application.Apartamento.Queries
{
    public class GetApartamentoOcupadosQuery : IRequest<IEnumerable<Domain.Entities.Apartamentos>>
    {
        public class GetApartamentoOcupadosQueryHandler : IRequestHandler<GetApartamentoOcupadosQuery, IEnumerable<Domain.Entities.Apartamentos>>
        {
            private IUnitOfWork _unitOfWork;

            public GetApartamentoOcupadosQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<IEnumerable<Domain.Entities.Apartamentos>> Handle(GetApartamentoOcupadosQuery request, CancellationToken cancellationToken)
            {
                var apartamentosOcupados = await _unitOfWork.Apartamento.GetApartamentoOcupadosAsync();
                return apartamentosOcupados;
            }
        }
        
    }
}