
using Hotel.Application.Common.PagedResult;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Reserva.Queries
{
    public class GetFilteredApartamentosReservadoQuery : IRequest<PagedList<ApartamentosReservado>>
    {
        public Domain.Interface.Shared.PaginationFilter paginationFilter { get; set; }

        public class GetFilteredApartamentosReservadoQueryHandler : IRequestHandler<GetFilteredApartamentosReservadoQuery, PagedList<ApartamentosReservado>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetFilteredApartamentosReservadoQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<PagedList<ApartamentosReservado>> Handle(GetFilteredApartamentosReservadoQuery request, CancellationToken cancellationToken)
            {
               /*  var paginatedResult = await _unitOfWork.Reservas.GetFilteredApartamentosReservadosquery(request.PaginationFilter);
                return new PagedList<ApartamentosReservado>(
                    paginatedResult.ToList(),
                    paginatedResult.TotalCount,
                    paginatedResult.CurrentPage,
                    paginatedResult.PageSize); */

 var aux = await PagedList<Domain.Entities.ApartamentosReservado>.ToPagedList((IQueryable<Domain.Entities.ApartamentosReservado>)
                                            _unitOfWork.Reservas.GetFilteredAsync(request.paginationFilter)
                                            ,request.paginationFilter.PageNumber
                                            ,request.paginationFilter.PageSize,cancellationToken);
                  return aux;

            }
        }
    }
}