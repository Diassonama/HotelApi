using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common.PagedResult;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.FacturaEmpresa.Queries
{
    public class GetFilteredQuery: IRequest<PagedList<Domain.Entities.FacturaEmpresa>>
    {
         public Domain.Interface.Shared.PaginationFilter  paginationFilter  { get; set; }
        public class GetFilterdQueryHandler : IRequestHandler<GetFilteredQuery, PagedList<Domain.Entities.FacturaEmpresa>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetFilterdQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<PagedList<Domain.Entities.FacturaEmpresa>> Handle(GetFilteredQuery request, CancellationToken cancellationToken)
            {
                var aux = await PagedList<Domain.Entities.FacturaEmpresa>.ToPagedList(_unitOfWork.Factura.GetFilteredAsync(request.paginationFilter), 
                                                        request.paginationFilter.PageNumber,request.paginationFilter.PageSize,cancellationToken);
                return aux;
            }
        }
    }
}