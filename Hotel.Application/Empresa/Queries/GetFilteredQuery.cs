using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common.PagedResult;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Empresa.Queries
{
    public class GetFilteredQuery:IRequest<PagedList<Domain.Entities.Empresa>>
    {
         public Domain.Interface.Shared.PaginationFilter paginationFilter { get; set; }
        public class GetFilteredQueryHandler : IRequestHandler<GetFilteredQuery, PagedList<Domain.Entities.Empresa>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetFilteredQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<PagedList<Domain.Entities.Empresa>> Handle(GetFilteredQuery request, CancellationToken cancellationToken)
            {
                var aux = await PagedList<Domain.Entities.Empresa>.ToPagedList((IQueryable<Domain.Entities.Empresa>)_unitOfWork.Empresa.GetFilteredAsync(request.paginationFilter),request.paginationFilter.PageNumber,request.paginationFilter.PageSize,cancellationToken);
                       //            var aux = await PagedList<Pais>.ToPagedList((IQueryable<Pais>)_unitOfWork.Paises.GetFilteredAsync(request.paginationFilter),request.paginationFilter.PageNumber,request.paginationFilter.PageSize,cancellationToken);
            
            return aux;
            }
        }
    }
}