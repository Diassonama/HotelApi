using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Application.PlanoDeConta.Queries
{
    public class GetPlanoDeContaByIdQuery : IRequest<Domain.Entities.PlanoDeConta>
    {
        public int Id { get; set; }

        public class GetPlanoDeContaByIdQueryHandler : IRequestHandler<GetPlanoDeContaByIdQuery, Domain.Entities.PlanoDeConta>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetPlanoDeContaByIdQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<Domain.Entities.PlanoDeConta> Handle(GetPlanoDeContaByIdQuery request, CancellationToken cancellationToken)
            {
                return await _unitOfWork.PlanoDeConta.Get(request.Id);
            }
        }
    }
}
