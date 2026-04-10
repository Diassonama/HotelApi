using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Interface;
using MediatR;
using Serilog;

namespace Hotel.Application.EmpresaSaldo.Queries
{
    public class GetSaldoAtualQuery: IRequest<decimal>
    {
        public int EmpresaId { get; set; }


         public class GetSaldoAtualQueryHandler : IRequestHandler<GetSaldoAtualQuery, decimal>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetSaldoAtualQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<decimal> Handle(GetSaldoAtualQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Buscando saldo atual da empresa {EmpresaId}", request.EmpresaId);
                return await _unitOfWork.EmpresaSaldo.GetSaldoAtualAsync(request.EmpresaId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar saldo atual da empresa {EmpresaId}", request.EmpresaId);
                return 0;
            }
        }
    }
    }
}