using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common.PagedResult;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.TipoApartamento.Queries
{
    public class GetFilteredTipoApartamentoQuery : IRequest<PagedList<Domain.Entities.TipoApartamento>>
    {
        public Domain.Interface.Shared.PaginationFilter  paginationFilter  { get; set; }
        public class GetFilteredTipoApartamentoQueryHandler : IRequestHandler<GetFilteredTipoApartamentoQuery, PagedList<Domain.Entities.TipoApartamento>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetFilteredTipoApartamentoQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<PagedList<Domain.Entities.TipoApartamento>> Handle(GetFilteredTipoApartamentoQuery request, CancellationToken cancellationToken)
            {
                var aux = await PagedList<Domain.Entities.TipoApartamento>.ToPagedList((IQueryable<Domain.Entities.TipoApartamento>)
                                            _unitOfWork.TipoApartamento.GetFilteredAsync(request.paginationFilter)
                                            ,request.paginationFilter.PageNumber
                                            ,request.paginationFilter.PageSize,cancellationToken);
                  return aux;
            }
        }
    }
}