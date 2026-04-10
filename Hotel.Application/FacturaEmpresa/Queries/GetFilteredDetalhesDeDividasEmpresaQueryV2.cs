using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common.PagedResult;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.FacturaEmpresa.Queries
{
    public class GetFilteredDetalhesDeDividasEmpresaQueryV2 : IRequest<PagedList<Domain.DTOs.FacturaEmpresaDetalhesDto>>
    {
        public Domain.Interface.Shared.PaginationFilter paginationFilter { get; set; }

    }   
    public class GetFilteredDetalhesDeDividasEmpresaQueryV2Handler : IRequestHandler<GetFilteredDetalhesDeDividasEmpresaQueryV2, PagedList<Domain.DTOs.FacturaEmpresaDetalhesDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFilteredDetalhesDeDividasEmpresaQueryV2Handler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<Domain.DTOs.FacturaEmpresaDetalhesDto>> Handle(GetFilteredDetalhesDeDividasEmpresaQueryV2 request, CancellationToken cancellationToken)
        {
            return await PagedList<Domain.DTOs.FacturaEmpresaDetalhesDto>.ToPagedList(_unitOfWork.Factura.GetFilteredDetalhesDeDividasEmpresaAsyncV2(request.paginationFilter, request.paginationFilter.EmpresaId),
            request.paginationFilter.PageNumber,
            request.paginationFilter.PageSize, cancellationToken
            );
        }
    }
}