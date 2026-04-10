using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.DTOs.TipoHospedagem;
using Hotel.Application.Responses;
using Hotel.Application.TipoHospedagem.Queries;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.ComponentModel.DataAnnotations;
using Hotel.Application.TipoHospedagem.Commands;

namespace Hotel.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de tipos de hospedagem
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TipoHospedagemController : ApiControllerBase
    {

        readonly IUnitOfWork _unitOfWork;
        public TipoHospedagemController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Obter todos os tipos de hospedagem
        /// </summary>
        /// <returns>Lista de tipos de hospedagem</returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<BaseCommandResponse> Get()
        {
            return await Mediator.Send(new GetTipoHospedagemQuery());
        }

        /// <summary>
        /// Buscar valor da diária baseado no tipo de hospedagem e parâmetros
        /// </summary>
        /// <param name="request">Parâmetros para cálculo da diária</param>
        /// <returns>Valor calculado da diária</returns>
        [HttpPost("buscar-diaria")]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> BuscarDiaria([FromBody] BuscaDiariaRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "Dados da requisição são obrigatórios",
                        Errors = new List<string> { "Request body não pode ser nulo" }
                    });
                }

                // Validar modelo
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = errors
                    });
                }

                // Validações específicas de negócio
                if (request.TipoHospedagem?.ToUpper() == "HORA" && !request.Hora.HasValue)
                {
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "Para hospedagem por hora, o parâmetro 'Hora' é obrigatório",
                        Errors = new List<string> { "Hora deve ser informada para tipo HORA" }
                    });
                }

                var query = new BuscaDiariaQuery(request);
                var response = await Mediator.Send(query);

                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseCommandResponse
                {
                    Success = false,
                    Message = "Erro interno do servidor",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Buscar valor da diária via GET (parâmetros na URL)
        /// </summary>
        /// <param name="tipoApartamento">Código do tipo de apartamento</param>
        /// <param name="dias">Número de dias/hóspedes</param>
        /// <param name="tipoHospedagem">Tipo de hospedagem (DIARIA, HORA, NOITE, ESPECIAL)</param>
        /// <param name="hora">Número de horas (obrigatório para tipo HORA)</param>
        /// <param name="dataReferencia">Data de referência (opcional, padrão: hoje)</param>
        /// <returns>Valor calculado da diária</returns>
        [HttpGet("buscar-diaria")]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> BuscarDiariaGet(
            [FromQuery][Required] int tipoApartamento,
            [FromQuery][Required][Range(1, 4)] int numeroDeHospedes,
            [FromQuery][Required] string tipoHospedagem,
            [FromQuery] int? hora = null,
            [FromQuery] DateTime? dataReferencia = null)
        {
            try
            {
                // Validar parâmetros
                if (tipoApartamento <= 0)
                {
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "Tipo de apartamento deve ser maior que zero"
                    });
                }

                if (string.IsNullOrWhiteSpace(tipoHospedagem))
                {
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "Tipo de hospedagem é obrigatório"
                    });
                }

                if (tipoHospedagem.ToUpper() == "HORA" && !hora.HasValue)
                {
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "Para hospedagem por hora, o parâmetro 'hora' é obrigatório"
                    });
                }

                var query = new BuscaDiariaQuery(tipoApartamento, numeroDeHospedes, tipoHospedagem, hora, dataReferencia);
                var response = await Mediator.Send(query);

                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseCommandResponse
                {
                    Success = false,
                    Message = "Erro interno do servidor",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Obter tipos de hospedagem disponíveis
        /// </summary>
        /// <returns>Lista dos tipos de hospedagem disponíveis</returns>
        [HttpGet("tipos-hospedagem")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterTiposHospedagem()
        {


            var tipos = await _unitOfWork.TipoHospedagem.GetAllAsync();
               
              
           

            return Ok(new BaseCommandResponse
            {
                Success = true,
                Message = "Tipos de hospedagem obtidos com sucesso",
                Data = tipos
            });
        }
        
  [HttpPost]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CriarTipoHospedagem([FromBody] CreateTipoHospedagemRequest request)
        {
            try
            {
                Log.Information("Recebido request para criar tipo de hospedagem: {Descricao}, Valor: {Valor}", 
                    request?.Descricao ?? "N/A", request?.Valor ?? 0);

                // ✅ VALIDAÇÃO 1: Request não nulo
                if (request == null)
                {
                    Log.Warning("Request nulo recebido para criar tipo de hospedagem");
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "Dados da requisição são obrigatórios",
                        Errors = new List<string>{ "Request body não pode ser nulo" }
                    });
                }

                // ✅ VALIDAÇÃO 2: Modelo válido
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    Log.Warning("Modelo inválido para criar tipo de hospedagem: {Errors}", 
                        string.Join(", ", errors));
                    
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = errors
                    });
                }

                // ✅ VALIDAÇÕES ESPECÍFICAS DE NEGÓCIO
                var validationErrors = ValidarRegrasNegocio(request);
                if (validationErrors.Any())
                {
                    Log.Warning("Validação de negócio falhou: {Errors}", string.Join(", ", validationErrors));
                    
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "Regras de negócio violadas",
                        Errors = validationErrors
                    });
                }

                // ✅ EXECUTAR COMMAND
                var command = new CreateTipoHospedagemCommand(request);
                var response = await Mediator.Send(command);

                if (response.Success)
                {
                    Log.Information("Tipo de hospedagem criado com sucesso: {Descricao}, ID: {Id}", 
                        request.Descricao, response.Data);
                    
                    return CreatedAtAction(
                        nameof(ObterTipoHospedagemPorId),
                        new { id = 4},
                        response
                    );
                }
                else
                {
                    Log.Warning("Falha ao criar tipo de hospedagem: {Message}", response.Message);
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro inesperado ao criar tipo de hospedagem: {Descricao}", 
                    request?.Descricao ?? "N/A");
                
                return StatusCode(500, new BaseCommandResponse
                {
                    Success = false,
                    Message = "Erro interno do servidor",
                    Errors = new List<string> { "Ocorreu um erro inesperado. Tente novamente." }
                });
            }
        }

  /// <summary>
        /// Obter tipo de hospedagem por ID
        /// </summary>
        /// <param name="id">ID do tipo de hospedagem</param>
        /// <returns>Dados do tipo de hospedagem</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterTipoHospedagemPorId(int id)
        {
            try
            {
                Log.Information("Buscando tipo de hospedagem por ID: {Id}", id);

                if (id <= 0)
                {
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "ID deve ser maior que zero"
                    });
                }

                var query = new GetTipoHospedagemByIdQuery(id);
                var response = await Mediator.Send(query);

                if (response.Success)
                {
                    return Ok(response);
                }
                else
                {
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar tipo de hospedagem por ID: {Id}", id);
                
                return StatusCode(500, new BaseCommandResponse
                {
                    Success = false,
                    Message = "Erro interno do servidor"
                });
            }
        }

        /// <summary>
        /// Obter todos os tipos de hospedagem
        /// </summary>
        /// <param name="incluirHospedagens">Se deve incluir hospedagens relacionadas</param>
        /// <returns>Lista de tipos de hospedagem</returns>
        [HttpGet("todos")]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObterTiposHospedagem([FromQuery] bool incluirHospedagens = false)
        {
            try
            {
                Log.Information("Buscando tipos de hospedagem. IncluirHospedagens: {IncluirHospedagens}", 
                    incluirHospedagens);

                var query = new GetTiposHospedagemQuery(incluirHospedagens);
                var response = await Mediator.Send(query);

                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao buscar tipos de hospedagem");
                
                return StatusCode(500, new BaseCommandResponse
                {
                    Success = false,
                    Message = "Erro interno do servidor"
                });
            }
        }

        /// <summary>
        /// Atualizar um tipo de hospedagem existente
        /// </summary>
        /// <param name="id">ID do tipo de hospedagem a ser atualizado</param>
        /// <param name="request">Dados atualizados do tipo de hospedagem</param>
        /// <returns>Tipo de hospedagem atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarTipoHospedagem(int id, [FromBody] UpdateTipoHospedagemRequest request)
        {
            try
            {
                Log.Information("Recebido request para atualizar tipo de hospedagem: ID={Id}, Descrição={Descricao}", 
                    id, request?.Descricao ?? "N/A");

                // ✅ VALIDAÇÃO 1: Request não nulo
                if (request == null)
                {
                    Log.Warning("Request nulo recebido para atualizar tipo de hospedagem: ID={Id}", id);
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "Dados da requisição são obrigatórios",
                        Errors = new List<string> { "Request body não pode ser nulo" }
                    });
                }

                // ✅ VALIDAÇÃO 2: ID consistente
                if (id != request.Id)
                {
                    Log.Warning("ID inconsistente entre URL e body: URL={UrlId}, Body={BodyId}", id, request.Id);
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "ID da URL deve ser igual ao ID do corpo da requisição",
                        Errors = new List<string> { $"URL ID: {id}, Body ID: {request.Id}" }
                    });
                }

                // ✅ VALIDAÇÃO 3: ID válido
                if (id <= 0)
                {
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "ID deve ser maior que zero"
                    });
                }

                // ✅ VALIDAÇÃO 4: Modelo válido
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    Log.Warning("Modelo inválido para atualizar tipo de hospedagem: {Errors}", 
                        string.Join(", ", errors));
                    
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "Dados inválidos",
                        Errors = errors
                    });
                }

                // ✅ VALIDAÇÕES ESPECÍFICAS DE NEGÓCIO
                var validationErrors = ValidarRegrasNegocioUpdate(request);
                if (validationErrors.Any())
                {
                    Log.Warning("Validação de negócio falhou: {Errors}", string.Join(", ", validationErrors));
                    
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "Regras de negócio violadas",
                        Errors = validationErrors
                    });
                }

                // ✅ EXECUTAR COMMAND
                var command = new UpdateTipoHospedagemCommand(request);
                var response = await Mediator.Send(command);

                if (response.Success)
                {
                    Log.Information("Tipo de hospedagem atualizado com sucesso: ID={Id}, Descrição={Descricao}", 
                        id, request.Descricao);
                    
                    return Ok(response);
                }
                else
                {
                    Log.Warning("Falha ao atualizar tipo de hospedagem: {Message}", response.Message);
                    
                    // Verificar se é erro de não encontrado
                    if (response.Message.Contains("não encontrado"))
                    {
                        return NotFound(response);
                    }
                    
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro inesperado ao atualizar tipo de hospedagem: ID={Id}", id);
                
                return StatusCode(500, new BaseCommandResponse
                {
                    Success = false,
                    Message = "Erro interno do servidor",
                    Errors = new List<string> { "Ocorreu um erro inesperado. Tente novamente." }
                });
            }
        }

        /// <summary>
        /// ✅ ENDPOINT ALTERNATIVO: PUT sem ID na URL (mantém compatibilidade)
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AtualizarTipoHospedagem([FromBody] UpdateTipoHospedagemRequest request)
        {
            try
            {
                Log.Information("Recebido request para atualizar tipo de hospedagem (sem ID na URL)");

                if (request == null)
                {
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "Dados da requisição são obrigatórios"
                    });
                }

                if (request.Id <= 0)
                {
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "ID deve ser informado no corpo da requisição e ser maior que zero"
                    });
                }

                // Redirecionar para o método com ID na URL
                return await AtualizarTipoHospedagem(request.Id, request);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao processar request de atualização sem ID na URL");
                return StatusCode(500, new BaseCommandResponse
                {
                    Success = false,
                    Message = "Erro interno do servidor"
                });
            }
        }

        /// <summary>
        /// Validações específicas de regras de negócio para atualização
        /// </summary>
        private List<string> ValidarRegrasNegocioUpdate(UpdateTipoHospedagemRequest request)
        {
            var errors = new List<string>();

            try
            {
                // Validar ID
                if (request.Id <= 0)
                {
                    errors.Add("ID deve ser maior que zero");
                }

                // Validar se a descrição não contém apenas espaços
                if (string.IsNullOrWhiteSpace(request.Descricao))
                {
                    errors.Add("Descrição não pode conter apenas espaços em branco");
                }
                else if (request.Descricao.Trim().Length < 3)
                {
                    errors.Add("Descrição deve ter pelo menos 3 caracteres");
                }
                else if (request.Descricao.Length > 100)
                {
                    errors.Add("Descrição deve ter no máximo 100 caracteres");
                }

                // Validar se o valor não é muito baixo
                if (request.Valor > 0 && request.Valor < 1.0f)
                {
                    errors.Add("Valor deve ser pelo menos R$ 1,00");
                }

                // Validar valor obrigatório
                if (request.Valor <= 0)
                {
                    errors.Add("Valor deve ser maior que zero");
                }

                // Validar se a descrição não tem caracteres especiais inadequados
                if (!string.IsNullOrEmpty(request.Descricao))
                {
                    var caracteresInvalidos = new[] { '<', '>', '&', '"', '\'' };
                    if (request.Descricao.IndexOfAny(caracteresInvalidos) >= 0)
                    {
                        errors.Add("Descrição não pode conter caracteres especiais como <, >, &, \", '");
                    }
                }

                // Validar caracteres em sequência
                if (!string.IsNullOrEmpty(request.Descricao))
                {
                    // Verificar se não tem mais de 3 caracteres iguais em sequência
                    for (int i = 0; i <= request.Descricao.Length - 3; i++)
                    {
                        if (request.Descricao[i] == request.Descricao[i + 1] && 
                            request.Descricao[i + 1] == request.Descricao[i + 2])
                        {
                            errors.Add("Descrição não pode ter mais de 2 caracteres iguais em sequência");
                            break;
                        }
                    }
                }

                Log.Information("Validação de regras de negócio (update) concluída. Erros: {ErrorCount}", errors.Count);
                return errors;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro durante validação de regras de negócio (update)");
                errors.Add("Erro na validação das regras de negócio");
                return errors;
            }
        }

        /// <summary>
        /// Validações específicas de regras de negócio
        /// </summary>
        private List<string> ValidarRegrasNegocio(CreateTipoHospedagemRequest request)
        {
            var errors = new List<string>();

            try
            {
                // Validar se a descrição não contém apenas espaços
                if (string.IsNullOrWhiteSpace(request.Descricao))
                {
                    errors.Add("Descrição não pode conter apenas espaços em branco");
                }

                // Validar se o valor não é muito baixo
                if (request.Valor > 0 && request.Valor < 1.0f)
                {
                    errors.Add("Valor deve ser pelo menos R$ 1,00");
                }

                // Validar se a descrição não tem caracteres especiais inadequados
                if (!string.IsNullOrEmpty(request.Descricao))
                {
                    var caracteresInvalidos = new[] { '<', '>', '&', '"', '\'' };
                    if (request.Descricao.IndexOfAny(caracteresInvalidos) >= 0)
                    {
                        errors.Add("Descrição não pode conter caracteres especiais como <, >, &, \", '");
                    }
                }

                Log.Information("Validação de regras de negócio concluída. Erros: {ErrorCount}", errors.Count);
                return errors;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro durante validação de regras de negócio");
                errors.Add("Erro na validação das regras de negócio");
                return errors;
            }
        }
    }

    
}