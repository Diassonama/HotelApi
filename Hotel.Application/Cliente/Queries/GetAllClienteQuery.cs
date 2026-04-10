using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Cliente.Queries
{
    public class GetAllClienteQuery : IRequest<IEnumerable<Domain.Entities.Cliente>>
    {
        public class GetAllClienteQueryHandler : IRequestHandler<GetAllClienteQuery, IEnumerable<Domain.Entities.Cliente>>
        {
            private IUnitOfWork _unitOfWork;

            public GetAllClienteQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<IEnumerable<Domain.Entities.Cliente>> Handle(GetAllClienteQuery request, CancellationToken cancellationToken)
            {
                var clientes = await _unitOfWork.clientes.GetAllAsync();
                return clientes;
            }
        }
    }
}