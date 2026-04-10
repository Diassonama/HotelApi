using Hotel.Application.DTOs.TipoHospedagem;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Hotel.Application.TipoHospedagem.Commands
{
    /// <summary>
    /// Command para atualizar um tipo de hospedagem
    /// </summary>
    public class UpdateTipoHospedagemCommand : IRequest<BaseCommandResponse>
    {
        public int Id { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public float Valor { get; set; }
        public bool Ativo { get; set; } = true;

        public UpdateTipoHospedagemCommand() { }

        public UpdateTipoHospedagemCommand(UpdateTipoHospedagemRequest request)
        {
            Id = request.Id;
            Descricao = request.Descricao?.Trim() ?? string.Empty;
            Valor = request.Valor;
            Ativo = request.Ativo;
        }
    }

    /// <summary>
    /// Handler para processar a atualização do tipo de hospedagem
    /// </summary>
    public class UpdateTipoHospedagemCommandHandler : IRequestHandler<UpdateTipoHospedagemCommand, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTipoHospedagemCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseCommandResponse> Handle(UpdateTipoHospedagemCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();

            try
            {
                Log.Information("Iniciando atualização de tipo de hospedagem: ID={Id}, Descrição={Descricao}",
                    request.Id, request.Descricao);

                // ✅ VALIDAÇÃO BÁSICA
                var validationErrors = ValidarDados(request);
                if (validationErrors.Any())
                {
                    response.Success = false;
                    response.Message = "Erros de validação encontrados";
                    response.Errors = validationErrors;
                    return response;
                }

                // ✅ BUSCAR ENTIDADE EXISTENTE
                var tipoHospedagemExistente = await _unitOfWork.TipoHospedagem.Get(request.Id);

                if (tipoHospedagemExistente == null)
                {
                    Log.Warning("Tipo de hospedagem não encontrado para atualização: ID={Id}", request.Id);

                    response.Success = false;
                    response.Message = $"Tipo de hospedagem com ID {request.Id} não encontrado";
                    response.Errors = new List<string> { "Registro não encontrado" };
                    return response;
                }

            
                // ✅ ATUALIZAR PROPRIEDADES DA ENTIDADE EXISTENTE
                AtualizarPropriedades(tipoHospedagemExistente, request);

                Log.Information("Propriedades atualizadas para TipoHospedagem ID={Id}", request.Id);

                // ✅ SALVAR ALTERAÇÕES
                await _unitOfWork.TipoHospedagem.Update(tipoHospedagemExistente);

                Log.Information("Tipo de hospedagem atualizado com sucesso: ID={Id}, Descrição={Descricao}", 
                    tipoHospedagemExistente.Id, tipoHospedagemExistente.Descricao);

                // ✅ PREPARAR RESPONSE
                response.Success = true;
                response.Message = "Tipo de hospedagem atualizado com sucesso";
                response.Data = MapearParaResponse(tipoHospedagemExistente);

                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao atualizar tipo de hospedagem: ID={Id}", request.Id);
                
                response.Success = false;
                response.Message = $"Erro ao atualizar tipo de hospedagem: {ex.Message}";
                response.Errors = new List<string> { ex.Message };
                
                return response;
            }
        }

        /// <summary>
        /// Atualiza propriedades da entidade existente
        /// </summary>
        private void AtualizarPropriedades(Domain.Entities.TipoHospedagem entidadeExistente, UpdateTipoHospedagemCommand request)
        {
            try
            {
                Log.Debug("Atualizando propriedades da entidade TipoHospedagem ID={Id}", request.Id);

                entidadeExistente.Descricao = request.Descricao.Trim();
                entidadeExistente.Valor = request.Valor;

                // Atualizar campo Ativo se existir na entidade
                var propriedadeAtivo = entidadeExistente.GetType().GetProperty("Ativo");
                if (propriedadeAtivo != null && propriedadeAtivo.CanWrite)
                {
                    propriedadeAtivo.SetValue(entidadeExistente, request.Ativo);
                }

                // Atualizar timestamp de modificação se existir
                var propriedadeDataAtualizacao = entidadeExistente.GetType().GetProperty("DataAtualizacao");
                if (propriedadeDataAtualizacao != null && propriedadeDataAtualizacao.CanWrite)
                {
                    propriedadeDataAtualizacao.SetValue(entidadeExistente, DateTime.UtcNow);
                }

                Log.Debug("Propriedades atualizadas com sucesso para TipoHospedagem ID={Id}", request.Id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao atualizar propriedades da entidade TipoHospedagem ID={Id}", request.Id);
                throw;
            }
        }

        /// <summary>
        /// Validações básicas dos dados
        /// </summary>
        private List<string> ValidarDados(UpdateTipoHospedagemCommand request)
        {
            var errors = new List<string>();

            try
            {
                if (request.Id <= 0)
                    errors.Add("ID deve ser maior que zero");

                if (string.IsNullOrWhiteSpace(request.Descricao))
                    errors.Add("Descrição é obrigatória");
                else if (request.Descricao.Trim().Length < 3)
                    errors.Add("Descrição deve ter pelo menos 3 caracteres");
                else if (request.Descricao.Length > 100)
                    errors.Add("Descrição deve ter no máximo 100 caracteres");

                if (request.Valor <= 0)
                    errors.Add("Valor deve ser maior que zero");

                Log.Debug("Validação de dados concluída. Erros encontrados: {ErrorCount}", errors.Count);
                return errors;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro durante validação de dados");
                errors.Add("Erro na validação dos dados");
                return errors;
            }
        }

        /// <summary>
        /// Mapeia entidade para response
        /// </summary>
        private object MapearParaResponse(Domain.Entities.TipoHospedagem tipoHospedagem)
        {
            return new
            {
                Id = tipoHospedagem.Id,
                Descricao = tipoHospedagem.Descricao,
                Valor = tipoHospedagem.Valor,
                Ativo = true, // Assumindo ativo por padrão
                DataAtualizacao = DateTime.UtcNow
            };
        }
    }
}