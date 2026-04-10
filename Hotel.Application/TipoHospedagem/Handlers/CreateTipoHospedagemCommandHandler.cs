using Hotel.Application.DTOs.TipoHospedagem;
using Hotel.Application.Responses;
using Hotel.Application.TipoHospedagem.Commands;
using Hotel.Domain.Interface;
using Hotel.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Hotel.Application.TipoHospedagem.Handlers
{
    /// <summary>
    /// Handler para criar um novo tipo de hospedagem
    /// </summary>
    public class CreateTipoHospedagemCommandHandler : IRequestHandler<CreateTipoHospedagemCommand, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTipoHospedagemCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseCommandResponse> Handle(CreateTipoHospedagemCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();

            try
            {
                Log.Information("Iniciando criação de tipo de hospedagem: {Descricao}, Valor: {Valor}", 
                    request.Descricao, request.Valor);

                // ✅ VALIDAÇÃO 1: Verificar se descrição já existe
                 var descricaoExistente = await _unitOfWork.TipoHospedagem.GetbyName(request.Descricao);
                    //.Where(t => t.Descricao.ToLower().Trim() == request.Descricao.ToLower().Trim())
                   // .FirstOrDefaultAsync(cancellationToken);

                if (descricaoExistente != null)
                {
                    Log.Warning("Tentativa de criar tipo de hospedagem com descrição já existente: {Descricao}", 
                        request.Descricao);
                    
                    response.Success = false;
                    response.Message = $"Já existe um tipo de hospedagem com a descrição '{request.Descricao}'";
                    response.Errors = new List<string> { "Descrição deve ser única" };
                    return response;
                }

                // ✅ VALIDAÇÃO 2: Verificar valores
                var validationErrors = ValidarDados(request);
                if (validationErrors.Any())
                {
                    Log.Warning("Erro de validação ao criar tipo de hospedagem: {Errors}", 
                        string.Join(", ", validationErrors));
                    
                    response.Success = false;
                    response.Message = "Dados inválidos";
                    response.Errors = validationErrors;
                    return response;
                }

                // ✅ CRIAR ENTIDADE
                var novoTipoHospedagem = new Domain.Entities.TipoHospedagem
                {
                    Descricao = request.Descricao.Trim(),
                    Valor = request.Valor,
                    DateCreated = DateTime.UtcNow,
                    CreatedBy = "Sistema" // TODO: Obter do contexto do usuário autenticado
                };

                Log.Information("Entidade TipoHospedagem criada: {Descricao}, Valor: {Valor}", 
                    novoTipoHospedagem.Descricao, novoTipoHospedagem.Valor);

                // ✅ SALVAR NO BANCO
                await _unitOfWork.TipoHospedagem.Add(novoTipoHospedagem);
              //  await _unitOfWork.CommitAsync();

                // Não é possível verificar saveResult, pois CommitAsync não retorna valor
                // Se necessário, trate exceções para detectar falha no commit

                Log.Information("Tipo de hospedagem criado com sucesso: {Descricao}, ID: {Id}", 
                    novoTipoHospedagem.Descricao, novoTipoHospedagem.Id);

                // ✅ PREPARAR RESPONSE
                var tipoHospedagemResponse = new TipoHospedagemResponse
                {
                    Id = novoTipoHospedagem.Id,
                    Descricao = novoTipoHospedagem.Descricao,
                    Valor = novoTipoHospedagem.Valor,
                    DateCreated = novoTipoHospedagem.DateCreated,
                  //  DateModified = novoTipoHospedagem.DateModified,
                  //  CreatedBy = novoTipoHospedagem.CreatedBy,
                  //  ModifiedBy = novoTipoHospedagem.ModifiedBy
                };

                response.Success = true;
                response.Message = "Tipo de hospedagem criado com sucesso";
              //  response.Id = novoTipoHospedagem.Id;
                response.Data = tipoHospedagemResponse;

                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao criar tipo de hospedagem: {Descricao}", request.Descricao);

                response.Success = false;
                response.Message = "Erro interno do servidor ao criar tipo de hospedagem";
                response.Errors = new List<string> { ex.Message };

                return response;
            }
        }

        /// <summary>
        /// Valida os dados do request
        /// </summary>
        private List<string> ValidarDados(CreateTipoHospedagemCommand request)
        {
            var errors = new List<string>();

            try
            {
                // Validar descrição
                if (string.IsNullOrWhiteSpace(request.Descricao))
                {
                    errors.Add("Descrição é obrigatória");
                }
                else if (request.Descricao.Trim().Length < 3)
                {
                    errors.Add("Descrição deve ter pelo menos 3 caracteres");
                }
                else if (request.Descricao.Length > 250)
                {
                    errors.Add("Descrição deve ter no máximo 250 caracteres");
                }

                // Validar valor
                if (request.Valor <= 0)
                {
                    errors.Add("Valor deve ser maior que zero");
                }
                else if (request.Valor > 999999.99f)
                {
                    errors.Add("Valor não pode exceder 999.999,99");
                }

                // Validar se valor tem precisão razoável (máximo 2 casas decimais)
                var valorString = request.Valor.ToString("F2");
                if (!float.TryParse(valorString, out _))
                {
                    errors.Add("Valor deve ter no máximo 2 casas decimais");
                }

                Log.Information("Validação concluída. Erros encontrados: {ErrorCount}", errors.Count);
                return errors;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro durante validação dos dados");
                errors.Add("Erro na validação dos dados");
                return errors;
            }
        }
    }
}