
using AutoMapper;
using Hotel.Application.Apartamento.Base;
using Hotel.Domain.Interface;
using Hotel.Domain.Entities;
//using Hotel.Infrastruture.Persistence.Context;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Hotel.Application.Apartamentos.Queries
{
    public class GetAllApartamentoQuery: IRequest<IEnumerable<Domain.Entities.Apartamentos>>
    {
    public class GetAllApartamentoQueryHandler : IRequestHandler<GetAllApartamentoQuery, IEnumerable<Domain.Entities.Apartamentos>>
    {
       // private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private IUnitOfWork _unitOfWork;
            public GetAllApartamentoQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
            {
                //   this.repository = repository;
                // _logger = logger;
                _unitOfWork = unitOfWork;
                _mapper = mapper;
            }
            public async Task<IEnumerable<Domain.Entities.Apartamentos>> Handle(GetAllApartamentoQuery request, CancellationToken cancellationToken)
        { 
             await _unitOfWork.Apartamento.AtualizarSituacaoApartamentosAsync();
            var apartamentos  = await _unitOfWork.Apartamento.GetApartamentoAsync();
            return apartamentos;
            //  _mapper.Map<List<ApartamentoResultModel>>(await _unitOfWork. Apartamento.GetApartamentoAsync());
       //  return Task.FromResult(_mapper.Map<List<ApartamentoResultModel>>(_unitOfWork.Apartamento.GetApartamentoAsync()));
         //   throw new NotImplementedException();

        }
    }
    }
}