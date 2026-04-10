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
    /// Handler para buscar tipo de hospedagem por ID
    /// </summary>
    public class GetTipoHospedagemByIdQueryHandler : IRequestHandler<GetTipoHospedagemByIdQuery, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTipoHospedagemByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseCommandResponse> Handle(GetTipoHospedagemByIdQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();

            try
            {
                Log.Information("Buscando tipo de hospedagem por ID: {Id}", request.Id);

                var tipoHospedagem = await _unitOfWork.TipoHospedagem.Get(request.Id);
                   

                if (tipoHospedagem == null)
                {
                    Log.Warning("Tipo de hospedagem não encontrado: {Id}", request.Id);
                    
                    response.Success = false;
                    response.Message = $"Tipo de hospedagem com ID {request.Id} não encontrado";
                    return response;
                }

                var tipoHospedagemResponse = new TipoHospedagemResponse
                {
                    Id = tipoHospedagem.Id,
                    Descricao = tipoHospedagem.Descricao,
                    Valor = tipoHospedagem.Valor,
                    DateCreated = tipoHospedagem.DateCreated,
                  //  DateModified = tipoHospedagem.DateModified,
                    CreatedBy = tipoHospedagem.CreatedBy,
                 //   ModifiedBy = tipoHospedagem.ModifiedBy
                };

                Log.Information("Tipo de hospedagem encontrado: {Descricao}", tipoHospedagem.Descricao);

                response.Success = true;
                response.Message = "Tipo de hospedagem encontrado";
                response.Data = tipoHospedagemResponse;

                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar tipo de hospedagem por ID: {Id}", request.Id);

                response.Success = false;
                response.Message = "Erro interno do servidor";
                response.Errors = new List<string> { ex.Message };

                return response;
            }
        }
    }
}