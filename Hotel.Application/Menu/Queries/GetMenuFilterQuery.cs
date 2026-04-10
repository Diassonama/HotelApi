using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common.PagedResult;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Domain.Interface.Shared;
using MediatR;

namespace Hotel.Application.Menu.Queries
{
    public class GetMenuFilterQuery:IRequest<PagedList<AppMenu>>
    {
        public Domain.Interface.Shared.PaginationFilter paginationFilter  { get; set; }
        public class GetMenuFilterQueryHandler : IRequestHandler<GetMenuFilterQuery, PagedList<AppMenu>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetMenuFilterQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async  Task<PagedList<AppMenu>> Handle(GetMenuFilterQuery request, CancellationToken cancellationToken)
            {
              /*   return await PagedList<AppMenu>.ToPagedList((IQueryable<AppMenu>)
                _unitOfWork.Menu.GetFilteredQuery(request.paginationFilter )
                                ,request.paginationFilter.PageNumber
                                ,request.paginationFilter.PageSize, cancellationToken); */

                         return await PagedList<AppMenu>.ToPagedList(
                                            _unitOfWork.Menu.GetFilteredAsync(request.paginationFilter)
                                            ,request.paginationFilter.PageNumber
                                            ,request.paginationFilter.PageSize,cancellationToken);
                //  return aux;        
            }
        }
    }
}