using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common.Models;
using Hotel.Application.Common.PagedResult;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Domain.Interface.Shared;
using MediatR;

namespace Hotel.Application.Apartamento.Queries
{
    public class GetFilteredAparamentoQuery : IRequest<PagedList<Domain.Entities.Apartamentos>>
    {
       // public PaginationFilter paginationFilter { get; set; }
        public Domain.Interface.Shared.PaginationFilter paginationFilter{ get; set; }

        
        public class GetFilteredAparamentoQueryHandler : IRequestHandler<GetFilteredAparamentoQuery, PagedList<Domain.Entities.Apartamentos>>
        {
            private IUnitOfWork _unitOfWork;
            
            public GetFilteredAparamentoQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<PagedList<Domain.Entities.Apartamentos>> Handle(GetFilteredAparamentoQuery request, CancellationToken cancellationToken)
            {
                 var aux = await PagedList<Domain.Entities.Apartamentos>.ToPagedList((IQueryable<Domain.Entities.Apartamentos>)
                                            _unitOfWork.Apartamento.GetFilteredAsync(request.paginationFilter)
                                            ,request.paginationFilter.PageNumber
                                            ,request.paginationFilter.PageSize,cancellationToken);
                  return aux;

             
            }
            
        }
    }
}