using Hotel.Application.DTOs.TipoHospedagem;
using Hotel.Application.Responses;
using Hotel.Application.TipoHospedagem.Queries;
using Hotel.Domain.Enums;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;
using System.Threading.Tasks;

namespace Hotel.Application.TipoHospedagem.Handlers
{
    /// <summary>
    /// Handler para buscar valor da diária baseado no tipo de hospedagem
    /// </summary>
    public class BuscaDiariaQueryHandler : IRequestHandler<BuscaDiariaQuery, BaseCommandResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public BuscaDiariaQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseCommandResponse> Handle(BuscaDiariaQuery request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            
            try
            {
                Log.Information("Iniciando busca de diária: TipoApartamento={TipoApartamento}, Dias={Dias}, TipoHospedagem={TipoHospedagem}, Hora={Hora}", 
                    request.TipoApartamento, request.numeroDeHospedes, request.TipoHospedagem, request.Hora);

                // Buscar tipo de apartamento
                var tipoApartamento = await _unitOfWork.TipoApartamento.GetByIdAsync(request.TipoApartamento);

                if (tipoApartamento == null)
                {
                    Log.Warning("Tipo de apartamento não encontrado: {TipoApartamento}", request.TipoApartamento);
                    response.Success = false;
                    response.Message = $"Tipo de apartamento {request.TipoApartamento} não encontrado";
                    return response;
                }

                var dataReferencia = request.DataReferencia ?? DateTime.Now;
                var valorCalculado = await CalcularValorPorTipo(tipoApartamento, request, dataReferencia);

                var resultado = new BuscaDiariaResponse
                {
                    TipoApartamento = request.TipoApartamento,
                    TipoHospedagem = request.TipoHospedagem,
                    ValorCalculado = valorCalculado.Valor,
                    numeroDeHospedes = request.numeroDeHospedes,
                    Hora = request.Hora,
                    DataReferencia = dataReferencia,
                    DiaSemana = ObterNomeDiaSemana(dataReferencia),
                    DescricaoCalculo = valorCalculado.Descricao,
                    Encontrado = valorCalculado.Valor > 0
                };

                Log.Information("Valor calculado: {Valor} para {TipoHospedagem}", valorCalculado.Valor, request.TipoHospedagem);

                response.Success = true;
                response.Message = "Valor da diária calculado com sucesso";
                response.Data = resultado;

                return response;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar valor da diária: TipoApartamento={TipoApartamento}", request.TipoApartamento);
                
                response.Success = false;
                response.Message = "Erro interno do servidor ao calcular diária";
                response.Errors = new List<string> { ex.Message };
                
                return response;
            }
        }

        /// <summary>
        /// Calcula o valor da diária baseado no tipo de hospedagem
        /// </summary>
        private async Task<(decimal Valor, string Descricao)> CalcularValorPorTipo(dynamic tipoApartamento, BuscaDiariaQuery request, DateTime dataReferencia)
        {
            try
            {
                switch (request.TipoHospedagem.ToUpper())
                {
                    case "DIARIA":
                        return CalcularValorDiaria(tipoApartamento, request.numeroDeHospedes);

                    case "HORA":
                        return CalcularValorHora(tipoApartamento, request.Hora ?? 1);

                    case "NOITE":
                        return CalcularValorNoite(tipoApartamento);

                    case "ESPECIAL":
                        return CalcularValorEspecial(tipoApartamento, dataReferencia);

                    default:
                           // ✅ NOVA FUNCIONALIDADE: Buscar por descrição na tabela TipoHospedagem
                        Log.Information("Tipo de hospedagem não reconhecido nos padrões. Buscando na tabela TipoHospedagem: {TipoHospedagem}", request.TipoHospedagem);
                        return await CalcularValorTipoHospedagem(request.TipoHospedagem);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao calcular valor para tipo {TipoHospedagem}", request.TipoHospedagem);
                return (0, $"Erro no cálculo: {ex.Message}");
            }
        }

        /// <summary>
        /// Calcula valor para hospedagem por diária
        /// ✅ CORRIGIDO: Retorna decimal e converte valores de forma segura
        /// </summary>
        private (decimal Valor, string Descricao) CalcularValorDiaria(dynamic tipoApartamento, int numeroDeHospedes)
        {
            try
            {
                decimal valor = numeroDeHospedes switch
                {
                    1 => ConvertToDecimal(tipoApartamento.ValorDiariaSingle),
                    2 => ConvertToDecimal(tipoApartamento.ValorDiariaDouble),
                    3 => ConvertToDecimal(tipoApartamento.ValorDiariaTriple),
                    _ => ConvertToDecimal(tipoApartamento.ValorDiariaQuadruple)
                };

                string descricao = numeroDeHospedes switch
                {
                    1 => "Diária Single (1 hóspede)",
                    2 => "Diária Double (2 hóspedes)",
                    3 => "Diária Triple (3 hóspedes)",
                    _ => "Diária Quádruple (4+ hóspedes)"
                };

                Log.Information("Valor diária calculado: {Valor} para {Dias} dias - {Descricao}", valor, numeroDeHospedes, descricao);
                return (valor, descricao);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao calcular valor diária para {Dias} dias", numeroDeHospedes);
                return (0, $"Erro no cálculo da diária: {ex.Message}");
            }
        }


        /// <summary>
        /// ✅ NOVA FUNÇÃO: Calcula valor buscando na tabela TipoHospedagem pela descrição
        /// </summary>
        /// <param name="descricaoTipoHospedagem">Descrição do tipo de hospedagem</param>
        /// <returns>Valor e descrição encontrados na tabela TipoHospedagem</returns>
        private async Task<(decimal Valor, string Descricao)> CalcularValorTipoHospedagem(string descricaoTipoHospedagem)
        {
            try
            {
                Log.Information("Buscando valor do tipo de hospedagem pela descrição: {Descricao}", descricaoTipoHospedagem);

                if (string.IsNullOrWhiteSpace(descricaoTipoHospedagem))
                {
                    Log.Warning("Descrição do tipo de hospedagem é nula ou vazia");
                    return (0, "Descrição do tipo de hospedagem não informada");
                }

                // Buscar tipo de hospedagem pela descrição
                var tipoHospedagem = await _unitOfWork.TipoHospedagem.GetbyName(descricaoTipoHospedagem);

                if (tipoHospedagem == null)
                {
                    Log.Warning("Tipo de hospedagem não encontrado com a descrição: {Descricao}", descricaoTipoHospedagem);
                    return (0, $"Tipo de hospedagem '{descricaoTipoHospedagem}' não encontrado");
                }

                // Converter o valor float para decimal de forma segura
                var valor = ConvertFloatToDecimal(tipoHospedagem.Valor);
                var descricao = $"Valor do tipo '{tipoHospedagem.Descricao}' (ID: {tipoHospedagem.Id})";

                Log.Information("Valor encontrado na tabela TipoHospedagem: {Valor} para descrição '{Descricao}'", 
                    valor, tipoHospedagem.Descricao);

                return (valor, descricao);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar valor na tabela TipoHospedagem para descrição: {Descricao}", descricaoTipoHospedagem);
                return (0, $"Erro ao buscar tipo de hospedagem: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ NOVA FUNÇÃO ASSÍNCRONA: Calcula valor buscando na tabela TipoHospedagem pela descrição
        /// </summary>
        /// <param name="descricaoTipoHospedagem">Descrição do tipo de hospedagem</param>
        /// <returns>Valor e descrição encontrados na tabela TipoHospedagem</returns>
        private async Task<(decimal Valor, string Descricao)> CalcularValorTipoHospedagemAsync(string descricaoTipoHospedagem)
        {
            try
            {
                Log.Information("Buscando valor do tipo de hospedagem pela descrição (async): {Descricao}", descricaoTipoHospedagem);

                if (string.IsNullOrWhiteSpace(descricaoTipoHospedagem))
                {
                    Log.Warning("Descrição do tipo de hospedagem é nula ou vazia");
                    return (0, "Descrição do tipo de hospedagem não informada");
                }

                // Buscar tipo de hospedagem pela descrição (assíncrono)
                var tipoHospedagem = await _unitOfWork.TipoHospedagem.GetbyName(descricaoTipoHospedagem);

                if (tipoHospedagem == null)
                {
                    Log.Warning("Tipo de hospedagem não encontrado com a descrição: {Descricao}", descricaoTipoHospedagem);
                    return (0, $"Tipo de hospedagem '{descricaoTipoHospedagem}' não encontrado");
                }

                // Converter o valor float para decimal de forma segura
                var valor = ConvertFloatToDecimal(tipoHospedagem.Valor);
                var descricao = $"Valor do tipo '{tipoHospedagem.Descricao}' (ID: {tipoHospedagem.Id})";

                Log.Information("Valor encontrado na tabela TipoHospedagem (async): {Valor} para descrição '{Descricao}'", 
                    valor, tipoHospedagem.Descricao);

                return (valor, descricao);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar valor na tabela TipoHospedagem (async) para descrição: {Descricao}", descricaoTipoHospedagem);
                return (0, $"Erro ao buscar tipo de hospedagem: {ex.Message}");
            }
        }

 private decimal ConvertFloatToDecimal(float floatValue)
        {
            try
            {
                Log.Debug("Convertendo float {Value} para decimal", floatValue);
                return Convert.ToDecimal(floatValue);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Erro ao converter float {Value} para decimal. Retornando 0.", floatValue);
                return 0m;
            }
        }
        /// <summary>
        /// Calcula valor para hospedagem por hora
        /// ✅ CORRIGIDO: Usa conversão segura de tipos
        /// </summary>
        private (decimal Valor, string Descricao) CalcularValorHora(dynamic tipoApartamento, int horas)
        {
            try
            {
                decimal valor = horas switch
                {
                    1 => ConvertToDecimal(tipoApartamento.ValorUmaHora),
                    2 => ConvertToDecimal(tipoApartamento.ValorDuasHora),
                    3 => ConvertToDecimal(tipoApartamento.ValortresHora),
                    _ => ConvertToDecimal(tipoApartamento.ValorQuatroHora)
                };

                string descricao = horas switch
                {
                    1 => "Valor por 1 hora",
                    2 => "Valor por 2 horas",
                    3 => "Valor por 3 horas",
                    _ => "Valor por 4+ horas"
                };

                Log.Information("Valor hora calculado: {Valor} para {Horas} horas - {Descricao}", valor, horas, descricao);
                return (valor, descricao);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao calcular valor hora para {Horas} horas", horas);
                return (0, $"Erro no cálculo por hora: {ex.Message}");
            }
        }

        /// <summary>
        /// Calcula valor para hospedagem noturna
        /// ✅ CORRIGIDO: Usa conversão segura de tipos
        /// </summary>
        private (decimal Valor, string Descricao) CalcularValorNoite(dynamic tipoApartamento)
        {
            try
            {
                decimal valor = ConvertToDecimal(tipoApartamento.ValorNoite);
                string descricao = "Valor para hospedagem noturna";

                Log.Information("Valor noite calculado: {Valor} - {Descricao}", valor, descricao);
                return (valor, descricao);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao calcular valor noite");
                return (0, $"Erro no cálculo noturno: {ex.Message}");
            }
        }

        /// <summary>
        /// Calcula valor especial baseado no dia da semana
        /// ✅ CORRIGIDO: Usa conversão segura de tipos
        /// </summary>
        private (decimal Valor, string Descricao) CalcularValorEspecial(dynamic tipoApartamento, DateTime dataReferencia)
        {
            try
            {
                var diaSemana = (int)dataReferencia.DayOfWeek;
                var nomeDia = ObterNomeDiaSemana(dataReferencia);

                // Ajustar para o padrão brasileiro (Domingo = 1, Segunda = 2, etc.)
                var diaSemanaBrasil = diaSemana == 0 ? 7 : diaSemana;

                decimal valor = diaSemanaBrasil switch
                {
                    1 => ConvertToDecimal(tipoApartamento.Segunda),   // Segunda-feira
                    2 => ConvertToDecimal(tipoApartamento.Terca),     // Terça-feira
                    3 => ConvertToDecimal(tipoApartamento.Quarta),    // Quarta-feira
                    4 => ConvertToDecimal(tipoApartamento.Quinta),    // Quinta-feira
                    5 => ConvertToDecimal(tipoApartamento.Sexta),     // Sexta-feira
                    6 => ConvertToDecimal(tipoApartamento.Sabado),    // Sábado
                    7 => ConvertToDecimal(tipoApartamento.Domingo),   // Domingo
                    _ => 0
                };

                string descricao = $"Valor especial para {nomeDia} ({dataReferencia:dd/MM/yyyy})";

                Log.Information("Valor especial calculado: {Valor} para {DiaSemana} - {Descricao}", valor, nomeDia, descricao);
                return (valor, descricao);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao calcular valor especial para {Data}", dataReferencia);
                return (0, $"Erro no cálculo especial: {ex.Message}");
            }
        }

        /// <summary>
        /// ✅ NOVO MÉTODO: Converte valores de forma segura para decimal
        /// Lida com null, double, float, decimal, string, etc.
        /// </summary>
        private decimal ConvertToDecimal(object value)
        {
            try
            {
                if (value == null || value == DBNull.Value)
                {
                    Log.Debug("Valor nulo encontrado, retornando 0");
                    return 0m;
                }

                // Se já é decimal, retorna diretamente
                if (value is decimal decimalValue)
                {
                    return decimalValue;
                }

                // Se é double, converte para decimal
                if (value is double doubleValue)
                {
                    Log.Debug("Convertendo double {Value} para decimal", doubleValue);
                    return Convert.ToDecimal(doubleValue);
                }

                // Se é float, converte para decimal
                if (value is float floatValue)
                {
                    Log.Debug("Convertendo float {Value} para decimal", floatValue);
                    return Convert.ToDecimal(floatValue);
                }

                // Se é int, converte para decimal
                if (value is int intValue)
                {
                    Log.Debug("Convertendo int {Value} para decimal", intValue);
                    return Convert.ToDecimal(intValue);
                }

                // Se é string, tenta parse
                if (value is string stringValue)
                {
                    Log.Debug("Tentando converter string '{Value}' para decimal", stringValue);
                    
                    if (string.IsNullOrEmpty(stringValue))
                        return 0m;

                    if (decimal.TryParse(stringValue, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal parsedValue))
                        return parsedValue;
                    
                    // Tentar com cultura brasileira
                    if (decimal.TryParse(stringValue, NumberStyles.Number, new CultureInfo("pt-BR"), out decimal parsedValueBR))
                        return parsedValueBR;
                }

                // Última tentativa: usar Convert.ToDecimal
                Log.Debug("Tentando conversão geral do tipo {Type} para decimal", value.GetType().Name);
                return Convert.ToDecimal(value);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Erro ao converter valor {Value} do tipo {Type} para decimal. Retornando 0.", 
                    value?.ToString() ?? "null", value?.GetType().Name ?? "null");
                return 0m;
            }
        }

        /// <summary>
        /// Obtém o nome do dia da semana em português
        /// </summary>
        private string ObterNomeDiaSemana(DateTime data)
        {
            try
            {
                var cultura = new CultureInfo("pt-BR");
                return cultura.DateTimeFormat.GetDayName(data.DayOfWeek);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Erro ao obter nome do dia da semana para {Data}", data);
                return data.DayOfWeek.ToString();
            }
        }
    }
}