using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common.PagedResult;
using Hotel.Domain.Dtos;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.FacturaEmpresa.Queries
{
    public class GetFilteredEmpresasComDividasQuery: IRequest<PagedList<FacturaEmpresaDto>>
    {
        public Domain.Interface.Shared.PaginationFilter  paginationFilter  { get; set; }
        public class GetFilteredEmpresasComDividasQueryHandler : IRequestHandler<GetFilteredEmpresasComDividasQuery, PagedList<FacturaEmpresaDto>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetFilteredEmpresasComDividasQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<PagedList<FacturaEmpresaDto>> Handle(GetFilteredEmpresasComDividasQuery request, CancellationToken cancellationToken)
            {
                return await PagedList<FacturaEmpresaDto>.ToPagedList(_unitOfWork.Factura.GetFilteredEmpresasComDividasAsync(request.paginationFilter)
                ,request.paginationFilter.PageNumber,request.paginationFilter.PageSize, cancellationToken);
            }
        }
    }
}