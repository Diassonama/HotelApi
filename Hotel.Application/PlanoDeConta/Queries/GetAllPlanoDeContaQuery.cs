using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Application.PlanoDeConta.Queries
{
    public class GetAllPlanoDeContaQuery : IRequest<IEnumerable<Domain.Entities.PlanoDeConta>>
    {
        public class GetAllPlanoDeContaQueryHandler : IRequestHandler<GetAllPlanoDeContaQuery, IEnumerable<Domain.Entities.PlanoDeConta>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetAllPlanoDeContaQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<IEnumerable<Domain.Entities.PlanoDeConta>> Handle(GetAllPlanoDeContaQuery request, CancellationToken cancellationToken)
            {
                return await _unitOfWork.PlanoDeConta.GetAll().ToListAsync(cancellationToken);
            }
        }
    }
}
