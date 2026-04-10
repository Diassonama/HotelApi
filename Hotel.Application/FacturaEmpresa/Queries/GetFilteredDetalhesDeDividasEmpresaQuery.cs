
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
    public class GetFilteredDetalhesDeDividasEmpresaQuery: IRequest<PagedList<Domain.Entities.FacturaEmpresa>>
    {
        public Domain.Interface.Shared.PaginationFilter  paginationFilter  { get; set; }
        
        public class GetFilteredDetalhesDeDividasEmpresaQueryHandler: IRequestHandler<GetFilteredDetalhesDeDividasEmpresaQuery, PagedList<Domain.Entities.FacturaEmpresa>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetFilteredDetalhesDeDividasEmpresaQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public  async Task<PagedList<Domain.Entities.FacturaEmpresa>> Handle(GetFilteredDetalhesDeDividasEmpresaQuery request, CancellationToken cancellationToken)
            {
                return await PagedList<Domain.Entities.FacturaEmpresa>.ToPagedList(_unitOfWork.Factura.GetFilteredDetalhesDeDividasEmpresaAsync(request.paginationFilter, request.paginationFilter.EmpresaId),
                request.paginationFilter.PageNumber,
                request.paginationFilter.PageSize, cancellationToken
                );
            }
        }
    }
}