using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common.PagedResult;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Paises.Queries
{
    public class GetFilteredPaisQuery: IRequest<PagedList<Pais>>
    {
         public Domain.Interface.Shared.PaginationFilter paginationFilter { get; set; }
        public class GetFilteredPaisQueryHandler : IRequestHandler<GetFilteredPaisQuery, PagedList<Pais>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetFilteredPaisQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<PagedList<Pais>> Handle(GetFilteredPaisQuery request, CancellationToken cancellationToken)
            {
                var aux = await PagedList<Pais>.ToPagedList((IQueryable<Pais>)_unitOfWork.Paises.GetFilteredAsync(request.paginationFilter),request.paginationFilter.PageNumber,request.paginationFilter.PageSize,cancellationToken);
                return aux;
            }
        }
    }
}