using FluentValidation;
using Hotel.Application.Responses;
using Hotel.Application.TipoApartamento.Base;
using Hotel.Domain.Interface;
using MediatR;
using Serilog;

namespace Hotel.Application.TipoApartamento.Commands
{
    /// <summary>
    /// Command para atualizar um tipo de apartamento
    /// </summary>
    public class UpdateTipoApartamentoCommand : TipoApartamentoCommandBase //IRequest<BaseCommandResponse>
    {
        public int Id { get; set; }
        public bool Ativo { get; set; } = true;

        public class UpdateTipoApartamentoCommandHandler : IRequestHandler<UpdateTipoApartamentoCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            // private readonly IValidator<UpdateTipoApartamentoCommand> _validator;


            public UpdateTipoApartamentoCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;

            }

            public async Task<BaseCommandResponse> Handle(UpdateTipoApartamentoCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();
                // var validateResult = await _validator.ValidateAsync(request);

                var validationErrors = ValidarDados(request);
                    if (validationErrors.Any())
                    {
                        response.Success = false;
                        response.Message = "Erros de validação encontrados";
                        response.Errors = validationErrors;
                        return response;
                    }

                Log.Information("Iniciando atualização de tipo de apartamento: ID={Id}, Nome={Nome}",
                    request.Id, request.Descricao);

                // ✅ VALIDAÇÃO 1: Verificar se o tipo de apartamento existe
                var tipoApartamentoExistente = await _unitOfWork.TipoApartamento.Get(request.Id);

                if (tipoApartamentoExistente == null)
                {
                    Log.Warning("Tipo de apartamento não encontrado para atualização: ID={Id}", request.Id);

                    response.Success = false;
                    response.Message = $"Tipo de apartamento com ID {request.Id} não encontrado";
                    response.Errors = new List<string> { "Registro não encontrado" };
                    return response;
                }


                try
                {
                    var tipoApartamento = new Domain.Entities.TipoApartamento(request.Id, request.Descricao, request.ValorDiariaSingle, request.ValorDiariaDouble, request.ValorDiariaTriple, request.ValorDiariaQuadruple, request.ValorUmaHora, request.ValorDuasHora, request.ValorTresHora, request.ValorQuatroHora, request.ValorNoite, request.Segunda, request.Terca, request.Quarta, request.Quinta, request.Sexta, request.Sabado, request.Domingo);

                    await _unitOfWork.TipoApartamento.Update(tipoApartamento);
                    response.Data = tipoApartamento;
                    response.Success = true;
                    response.Message = "Tipo de apartamento atualizado com sucesso";
                    //     };
                }
                catch (Exception ex)
                {
                    // Tratamento de exceções
                    response.Success = false;
                    response.Message = $"Erro ao atualizar tipo de apartamento: {ex.Message}";
                }
                //throw new NotImplementedException();
                return await Task.FromResult(response);

            }
               private List<string> ValidarDados(UpdateTipoApartamentoCommand request)
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

                    // Validar valores obrigatórios
                    var valores = new Dictionary<string, float>
                    {
                        { "Valor Diária Single", request.ValorDiariaSingle },
                        { "Valor Diária Double", request.ValorDiariaDouble },
                        { "Valor Diária Triple", request.ValorDiariaTriple },
                        { "Valor Diária Quádruple", request.ValorDiariaQuadruple },
                        { "Valor Uma Hora", request.ValorUmaHora },
                        { "Valor Duas Horas", request.ValorDuasHora },
                        { "Valor Três Horas", request.ValorTresHora },
                        { "Valor Quatro Horas", request.ValorQuatroHora },
                        { "Valor Noite", request.ValorNoite }
                    };

                    foreach (var valor in valores.Where(v => v.Value <= 0))
                    {
                        errors.Add($"{valor.Key} deve ser maior que zero");
                    }

                    // Validar progressão lógica dos valores
                    if (request.ValorDuasHora < request.ValorUmaHora)
                        errors.Add("Valor de duas horas deve ser maior ou igual ao valor de uma hora");

                    if (request.ValorTresHora < request.ValorDuasHora)
                        errors.Add("Valor de três horas deve ser maior ou igual ao valor de duas horas");

                    if (request.ValorQuatroHora < request.ValorTresHora)
                        errors.Add("Valor de quatro horas deve ser maior ou igual ao valor de três horas");

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

        }
    }
}


