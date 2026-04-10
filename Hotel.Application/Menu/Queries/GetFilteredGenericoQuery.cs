using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hotel.Application.Common.PagedResult;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Menu.Queries
{
    public class GetFilteredGenericoQuery: IRequest<PagedList<AppMenu>>
    {
         public Domain.Interface.Shared.PaginationFilter paginationFilter  { get; set; }
        public class GetFilteredGenericoQueryHandler : IRequestHandler<GetFilteredGenericoQuery, PagedList<AppMenu>>
        {
            private readonly IUnitOfWork _unitOfWork;
            
            public GetFilteredGenericoQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public Task<PagedList<AppMenu>> Handle(GetFilteredGenericoQuery request, CancellationToken cancellationToken)
            {
                //Expression<Func<AppMenu, bool>> filterExpression = null;
                
                return PagedList<AppMenu>.ToPagedList(_unitOfWork.Menu.GetFilteredGenAsync(request.paginationFilter)
                                                        ,request.paginationFilter.PageNumber
                                                        ,request.paginationFilter.PageSize,cancellationToken);
            }
        }
    }
}