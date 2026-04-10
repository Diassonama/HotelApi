using Hotel.Application.Common.PagedResult;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Reserva.Queries
{
    // Query to get filtered reservations with pagination;

public class GetFilteredReservaQuery: IRequest<PagedList<Domain.Entities.Reserva>>
{
    public Domain.Interface.Shared.PaginationFilter PaginationFilter { get; set; }

    public class GetFilteredReservaQueryHandler : IRequestHandler<GetFilteredReservaQuery, PagedList<Domain.Entities.Reserva>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetFilteredReservaQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<Domain.Entities.Reserva>> Handle(GetFilteredReservaQuery request, CancellationToken cancellationToken)
        {
            var paginatedResult = await _unitOfWork.Reservas.GetFilteredReservaquery(request.PaginationFilter);
            
            // Converte IPaginatedList para PagedList
            // IPaginatedList herda de List<T>, então os itens estão diretamente na lista
            return new PagedList<Domain.Entities.Reserva>(
                paginatedResult.ToList(),
                paginatedResult.TotalCount,
                paginatedResult.CurrentPage,
                paginatedResult.PageSize);
        }
    }
}

    
}
