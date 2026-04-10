using Hotel.Application.DTOs.TipoHospedagem;
using Hotel.Application.Responses;
using Hotel.Application.TipoHospedagem.Queries;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Hotel.Application.TipoHospedagem.Handlers
{
    /// <summary>
    /// Handler para buscar todos os tipos de hospedagem
    /// </summary>
    public class GetTiposHospedagemQueryHandler : IRequestHandler<GetTiposHospedagemQuery, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTiposHospedagemQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseCommandResponse> Handle(GetTiposHospedagemQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();

            try
            {
                Log.Information("Buscando todos os tipos de hospedagem. IncluirHospedagens: {IncluirHospedagens}", 
                    request.IncluirHospedagens);

                var query = await _unitOfWork.TipoHospedagem.GetAllAsync();

              /*   if (request.IncluirHospedagens)
                {
                    query = query.Include(t => t.Hospedagens);
                }

                var tiposHospedagem = await query
                    .OrderBy(t => t.Descricao)
                    .ToListAsync(cancellationToken); 

                var tiposHospedagemResponse = tiposHospedagem.Select(t => new TipoHospedagemResponse
                {
                    Id = t.Id,
                    Descricao = t.Descricao,
                    Valor = t.Valor,
                    DateCreated = t.DateCreated,
                    DateModified = t.DateModified,
                    CreatedBy = t.CreatedBy,
                    ModifiedBy = t.ModifiedBy
                }).ToList();*/

                Log.Information("Encontrados {Count} tipos de hospedagem", query.Count());

                response.Success = true;
                response.Message = $"Encontrados {query.Count()} tipos de hospedagem";
                response.Data = query;

                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar tipos de hospedagem");

                response.Success = false;
                response.Message = "Erro interno do servidor";
                response.Errors = new List<string> { ex.Message };

                return response;
            }
        }
    }
}