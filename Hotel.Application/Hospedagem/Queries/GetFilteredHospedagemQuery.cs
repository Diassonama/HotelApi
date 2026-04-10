using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common.PagedResult;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Hospedagem.Queries
{
    public class GetFilteredHospedagemQuery: IRequest<PagedList<Domain.Entities.Hospedagem>>
    {
         public Domain.Interface.Shared.PaginationFilter  paginationFilter  { get; set; }
        public class GetFilteredHospedagemQueryHandler : IRequestHandler<GetFilteredHospedagemQuery, PagedList<Domain.Entities.Hospedagem>>
        {
            private readonly IUnitOfWork  _unitOfWork;

            public GetFilteredHospedagemQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<PagedList<Domain.Entities.Hospedagem>> Handle(GetFilteredHospedagemQuery request, CancellationToken cancellationToken)
            {
                 var aux = await PagedList<Domain.Entities.Hospedagem>.ToPagedList((IQueryable<Domain.Entities.Hospedagem>)
                                            _unitOfWork.Hospedagem.GetFilteredAsync(request.paginationFilter)
                                            ,request.paginationFilter.PageNumber
                                            ,request.paginationFilter.PageSize,cancellationToken);
                  return aux;
            }
        }
    }
}