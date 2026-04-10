using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common.PagedResult;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Cliente.Queries
{
    public class GetFilteredClienteQuery: IRequest<PagedList<Domain.Entities.Cliente>>
    {
        public Domain.Interface.Shared.PaginationFilter paginationFilter { get; set; }
        public class GetFilteredClienteQueryHandler : IRequestHandler<GetFilteredClienteQuery, PagedList<Domain.Entities.Cliente>>
        {
            private readonly IUnitOfWork  _unitOfWork;
            public GetFilteredClienteQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<PagedList<Domain.Entities.Cliente>> Handle(GetFilteredClienteQuery request, CancellationToken cancellationToken)
            {
                var aux = await PagedList<Domain.Entities.Cliente>.ToPagedList((IQueryable<Domain.Entities.Cliente>)
                                            _unitOfWork.clientes.GetFilteredAsync(request.paginationFilter)
                                            ,request.paginationFilter.PageNumber
                                            ,request.paginationFilter.PageSize,cancellationToken);
                  return aux;
            }
        }
    }
}