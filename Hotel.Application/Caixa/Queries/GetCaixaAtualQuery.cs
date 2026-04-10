using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Caixa.Queries
{
    public class GetCaixaAtualQuery:IRequest<int>
    {
        public class GetCiaixaAtualHandler : IRequestHandler<GetCaixaAtualQuery, int>
        {
            private IUnitOfWork _unitOfWork;

            public GetCiaixaAtualHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public Task<int> Handle(GetCaixaAtualQuery request, CancellationToken cancellationToken)
            {
                return _unitOfWork.caixa.getCaixa();
            }
        }
    }
}