using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Interface;
using MediatR;
using Serilog;

namespace Hotel.Application.EmpresaSaldo.Queries
{
    public class VerificarSaldoSuficienteQuery: IRequest<bool>
    {
        public int EmpresaId { get; set; }
        public decimal Valor { get; set; }

public class VerificarSaldoSuficienteQueryHandler : IRequestHandler<VerificarSaldoSuficienteQuery, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public VerificarSaldoSuficienteQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(VerificarSaldoSuficienteQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Log.Information("Verificando saldo suficiente para empresa {EmpresaId}, Valor: {Valor}", 
                    request.EmpresaId, request.Valor);

                return await _unitOfWork.EmpresaSaldo.VerificarSaldoSuficienteAsync(request.EmpresaId, request.Valor);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao verificar saldo suficiente");
                return false;
            }
        }
    }

    }
}