using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.FacturaEmpresa.Queries
{
    public class GetFacturaEmpresaQuery : IRequest<BaseCommandResponse>
    {
        public class GetFacturaEmpresaQueryHandler : IRequestHandler<GetFacturaEmpresaQuery, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public GetFacturaEmpresaQueryHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(GetFacturaEmpresaQuery request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var facturaEmpresa = await _unitOfWork.Factura.GetFacturaEmpresaAsync();
                //resposta.Data = facturaEmpresa;
                try
                {
                    if (facturaEmpresa == null)
                    {
                        resposta.Success = false;
                        resposta.Message = "Não existe factura empresa";
                    }
                    else
                    {
                        resposta.Success = true;
                        resposta.Message = "Factura empresa encontrada";
                        resposta.Data = facturaEmpresa;
                    }
                }
                catch (Exception ex)
                {
                    resposta.Success = false;
                    resposta.Message = $"Erro ao carregar dados {ex.Message}";
                }

                return resposta;
            }
        }
    }
}