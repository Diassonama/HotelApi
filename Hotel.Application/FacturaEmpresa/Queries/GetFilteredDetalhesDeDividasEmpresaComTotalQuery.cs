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
    public class GetFilteredDetalhesDeDividasEmpresaComTotalQuery : IRequest<(PagedList<Domain.Entities.FacturaEmpresa> PaginatedData, float ValorTotal)>
    {
        public Domain.Interface.Shared.PaginationFilter paginationFilter { get; set; }

        public class GetFilteredDetalhesDeDividasEmpresaComTotalQueryHandler : IRequestHandler<GetFilteredDetalhesDeDividasEmpresaComTotalQuery, (PagedList<Domain.Entities.FacturaEmpresa> PaginatedData, float ValorTotal)>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetFilteredDetalhesDeDividasEmpresaComTotalQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<(PagedList<Domain.Entities.FacturaEmpresa> PaginatedData, float ValorTotal)> Handle(GetFilteredDetalhesDeDividasEmpresaComTotalQuery request, CancellationToken cancellationToken)
            {

                var (registros, valorTotal) = _unitOfWork.Factura.GetFilteredDetalhesDeDividasEmpresaComTotalAsync(request.paginationFilter, request.paginationFilter.EmpresaId);

                var paginatedData = await PagedList<Domain.Entities.FacturaEmpresa>.ToPagedList(
                    registros,
                    request.paginationFilter.PageNumber,
                    request.paginationFilter.PageSize,
                    cancellationToken
                );

                return (paginatedData, valorTotal);

            }

            /* 
            public async Task<(PagedList<Domain.Entities.FacturaEmpresa> PaginatedData, float ValorTotal)> Handle(GetFilteredDetalhesDeDividasEmpresaQuery request, CancellationToken cancellationToken)
{
    var (registros, valorTotal) = _unitOfWork.Factura.GetFilteredDetalhesDeDividasEmpresaComTotalAsync(request.paginationFilter, request.EmpresaId);

    var paginatedData = await PagedList<Domain.Entities.FacturaEmpresa>.ToPagedList(
        registros,
        request.paginationFilter.PageNumber,
        request.paginationFilter.PageSize,
        cancellationToken
    );

    return (paginatedData, valorTotal);
}

             */
        }
    }
}