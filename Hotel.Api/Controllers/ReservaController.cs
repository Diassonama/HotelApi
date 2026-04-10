using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Common.PagedResult;
using Hotel.Application.Reserva.Commands;
using Hotel.Application.Reserva.Models;
using Hotel.Application.Reserva.Queries;
using Hotel.Application.Responses;
using Hotel.Application;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ReservaController : ApiControllerBase
    {
        public ReservaController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        /// <summary>
        /// Obtém todas as reservas ativas
        /// </summary>
        /// <returns>Lista de reservas</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reserva>>> GetAllReservas()
        {
            var result = await Mediator.Send(new GetAllReservasQuery());
            return Ok(result);
        }
        [HttpGet("get-with-pagination")]
         public async Task<ActionResult<PagedList<Reserva>>> GetAllFiltro([FromQuery] GetFilteredReservaQuery query)
        {
            return await Mediator.Send(query);
        }

         [HttpGet("get-with-pagination2")]
         public async Task<ActionResult<PagedList<ApartamentosReservado>>> GetAllFiltro2([FromQuery] GetFilteredApartamentosReservadoQuery query)
        {
            return await Mediator.Send(query);
        }

        /// <summary>
        /// Obtém uma reserva específica por ID com todos os apartamentos
        /// </summary>
        /// <param name="id">ID da reserva</param>
        /// <returns>Reserva com apartamentos</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseCommandResponse>> GetReservaById(int id)
        {
            var result = await Mediator.Send(new GetReservaByIdQuery { Id = id });
            
            if (!result.Success)
            {
                return NotFound(result);
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Obtém reservas por período específico
        /// </summary>
        /// <param name="dataInicio">Data de início do período</param>
        /// <param name="dataFim">Data de fim do período</param>
        /// <returns>Lista de reservas no período</returns>
        [HttpGet("periodo")]
        public async Task<ActionResult<BaseCommandResponse>> GetReservasPorPeriodo(
            [FromQuery] DateTime dataInicio, 
            [FromQuery] DateTime dataFim)
        {
            var result = await Mediator.Send(new GetReservasPorPeriodoQuery 
            { 
                DataInicio = dataInicio, 
                DataFim = dataFim 
            });
            
            return Ok(result);
        }

        /// <summary>
        /// Obtém todas as reservas de um apartamento específico
        /// </summary>
        /// <param name="apartamentoId">ID do apartamento</param>
        /// <returns>Lista de reservas do apartamento</returns>
        [HttpGet("apartamento/{apartamentoId}")]
        public async Task<ActionResult<BaseCommandResponse>> GetReservasPorApartamento(int apartamentoId)
        {
            var result = await Mediator.Send(new GetReservasPorApartamentoQuery 
            { 
                ApartamentoId = apartamentoId 
            });
            
            return Ok(result);
        }

        /// <summary>
        /// Cria uma nova reserva com apartamentos usando o comando original
        /// </summary>
        /// <param name="command">Dados da reserva e apartamentos</param>
        /// <returns>ID da reserva criada</returns>
        [HttpPost]
        [ProducesResponseType(typeof(CreateReservaResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CreateReservaResponse>> CreateReserva([FromBody] CreateReservaCommand command)
        {
            try
            {
                var reservaId = await Mediator.Send(command);
                
                var response = new CreateReservaResponse
                {
                    Success = true,
                    Message = "Reserva criada com sucesso",
                    ReservaId = reservaId,
                    Data = new { reservaId }
                };

                return CreatedAtAction(nameof(GetReservaById), new { id = reservaId }, response);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Apartamento") && (ex.Message.Contains("não está disponível") || ex.Message.Contains("está ocupado") || ex.Message.Contains("já está reservado") || ex.Message.Contains("não foi encontrado")))
            {
                // ✅ ERRO DETALHADO PARA O USUÁRIO: Mostra mensagem específica de disponibilidade
                var errorResponse = new ErrorResponse
                {
                    Success = false,
                    Message = ex.Message, // Usar a mensagem detalhada que vem do comando
                    Errors = new[] { 
                        "💡 Sugestão: Tente selecionar outro apartamento ou período",
                        ex.Message 
                    }
                };

                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                // ✅ ERRO GERAL MELHORADO: Apresenta informação útil ao usuário
                var errorResponse = new ErrorResponse
                {
                    Success = false,
                    Message = "❌ Erro inesperado ao processar a reserva",
                    Errors = new[] { 
                        "🔧 Se o problema persistir, entre em contato com o suporte",
                        $"Detalhe técnico: {ex.Message}"
                    }
                };

                return BadRequest(errorResponse);
            }
        }

        /// <summary>
        /// Cria uma nova reserva usando o comando original (retorna ID da reserva)
        /// </summary>
        /// <param name="command">Dados da reserva e apartamentos</param>
        /// <returns>ID da reserva criada</returns>
        [HttpPost("create-original")]
        public async Task<ActionResult<object>> CreateReservaOriginal([FromBody] CreateReservaCommand command)
        {
            try
            {
                var reservaId = await Mediator.Send(command);
                
                return Ok(new 
                { 
                    success = true,
                    message = "Reserva criada com sucesso",
                    id = reservaId,
                    data = new { reservaId }
                });
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Apartamento") && (ex.Message.Contains("não está disponível") || ex.Message.Contains("está ocupado") || ex.Message.Contains("já está reservado") || ex.Message.Contains("não foi encontrado")))
            {
                // ✅ ERRO DETALHADO PARA O USUÁRIO: Apresenta mensagem específica de disponibilidade
                return BadRequest(new 
                { 
                    success = false,
                    message = ex.Message, // Usar a mensagem detalhada que vem do comando
                    errors = new[] { 
                        "💡 Sugestão: Verifique outros apartamentos ou datas disponíveis",
                        ex.Message 
                    }
                });
            }
            catch (Exception ex)
            {
                // ✅ ERRO GERAL MELHORADO: Informação útil ao usuário
                return BadRequest(new 
                { 
                    success = false,
                    message = "❌ Erro inesperado ao processar a reserva",
                    errors = new[] { 
                        "🔧 Entre em contato com o suporte se o problema persistir",
                        $"Detalhe técnico: {ex.Message}"
                    }
                });
            }
        }

        /// <summary>
        /// Cria uma nova reserva com resposta estruturada e mensagens de erro detalhadas
        /// </summary>
        /// <param name="command">Dados da reserva e apartamentos</param>
        /// <returns>Resultado detalhado da operação</returns>
        [HttpPost("create-with-details")]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseCommandResponse>> CreateReservaWithDetails([FromBody] CreateReservaWithResponseCommand command)
        {
            var result = await Mediator.Send(command);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
             return Created("", result);
            // ✅ CORREÇÃO: Verificação defensiva para extrair o ID da reserva
           /*  try
            {
                if (result.Data != null)
                {
                    var reservaId = ((dynamic)result.Data).reservaId;
                    return CreatedAtAction(nameof(GetReservaById), new { id = reservaId }, result);
                }
                else
                {
                    // Se não há Data, retorna Created sem location
                    return Created("", result);
                }
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                // Se não conseguir extrair o reservaId, retorna Created sem location
                return Created("", result);
            } */
        }

        /// <summary>
        /// Cria uma nova reserva usando o comando V2 (com validação avançada)
        /// </summary>
        /// <param name="command">Dados da reserva e apartamentos</param>
        /// <returns>Resultado detalhado da operação</returns>
        [HttpPost("v2")]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseCommandResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BaseCommandResponse>> CreateReservaV2([FromBody] CreateReservaV2Command command)
        {
            try
            {
                var result = await Mediator.Send(command);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                
                // ✅ CORREÇÃO: Verificação defensiva para extrair o ID da reserva
                try
                {
                    if (result.Data != null)
                    {
                        // CreateReservaV2Command retorna a entidade Reserva no Data
                        var reserva = (Hotel.Domain.Entities.Reserva)result.Data;
                        return CreatedAtAction(nameof(GetReservaById), new { id = reserva.Id }, result);
                    }
                    else
                    {
                        return Created("", result);
                    }
                }
                catch (Exception)
                {
                    // Se não conseguir converter, retorna Created sem location
                    return Created("", result);
                }
            }
            catch (Exception ex) when (ex.Message.Contains("Apartamento") && (ex.Message.Contains("não está disponível") || ex.Message.Contains("está ocupado") || ex.Message.Contains("já está reservado") || ex.Message.Contains("não foi encontrado")))
            {
                // ✅ Erro específico de disponibilidade de apartamento
                var errorResponse = new BaseCommandResponse
                {
                    Success = false,
                    Message = "Apartamento não disponível",
                    Errors = new List<string> { ex.Message }
                };

                return BadRequest(errorResponse);
            }
        }


         [HttpPost("update")]
        public async Task<ActionResult<Response>> UpdateReserva2([FromBody] UpdateReservaCommand command)
        {
            try
            {
                // Validação básica
                if (command == null)
                {
                    return BadRequest(new Response 
                    { 
                        Success = false, 
                        Message = "Dados da reserva não podem ser nulos.",
                        Data = null 
                    });
                }

                if (command.Id <= 0)
                {
                    return BadRequest(new Response 
                    { 
                        Success = false, 
                        Message = "ID da reserva deve ser maior que zero.",
                        Data = null 
                    });
                }

                // Validação de apartamentos
                if (command.ApartamentosReservados == null || !command.ApartamentosReservados.Any())
                {
                    return BadRequest(new Response 
                    { 
                        Success = false, 
                        Message = "A reserva deve conter pelo menos um apartamento.",
                        Data = null 
                    });
                }

                var result = await Mediator.Send(command);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new Response 
                { 
                    Success = false, 
                    Message = $"Erro ao processar a atualização da reserva: {ex.Message}",
                    Data = null 
                });
            }
        }        /// <summary>

        /// <summary>
        /// Adiciona apartamentos a uma reserva existente
        /// </summary>
        /// <param name="command">ID da reserva e apartamentos a adicionar</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("adicionar-apartamentos")]
        public async Task<ActionResult<BaseCommandResponse>> AddApartamentosToReserva([FromBody] AddApartamentosToReservaCommand command)
        {
            try
            {
                var result = await Mediator.Send(command);
                
                if (!result.Success)
                {
                    return BadRequest(result);
                }
                
                return Ok(result);
            }
            catch (Exception ex) when (ex.Message.Contains("Apartamento") && (ex.Message.Contains("não está disponível") || ex.Message.Contains("está ocupado") || ex.Message.Contains("já está reservado") || ex.Message.Contains("não foi encontrado")))
            {
                // ✅ Erro específico de disponibilidade de apartamento
                var errorResponse = new BaseCommandResponse
                {
                    Success = false,
                    Message = "Apartamento não disponível",
                    Errors = new List<string> { ex.Message }
                };

                return BadRequest(errorResponse);
            }
        }

        /// <summary>
        /// Cancela apartamentos reservados específicos
        /// </summary>
        /// <param name="command">IDs dos apartamentos reservados a cancelar</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("cancelar-apartamentos")]
        public async Task<ActionResult<BaseCommandResponse>> CancelApartamentosReservados([FromBody] CancelApartamentosReservadosCommand command)
        {
            var result = await Mediator.Send(command);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }

        /// <summary>
        /// Atualiza uma reserva existente
        /// </summary>
        /// <param name="id">ID da reserva</param>
        /// <param name="command">Dados atualizados da reserva</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut]
        public async Task<ActionResult<Response>> UpdateReserva([FromBody] UpdateReservaCommand command)
        {
            try
            {
                // Validação básica
                if (command == null)
                {
                    return BadRequest(new Response 
                    { 
                        Success = false, 
                        Message = "Dados da reserva não podem ser nulos.",
                        Data = null 
                    });
                }

                if (command.Id <= 0)
                {
                    return BadRequest(new Response 
                    { 
                        Success = false, 
                        Message = "ID da reserva deve ser maior que zero.",
                        Data = null 
                    });
                }

                // Validação de apartamentos
                if (command.ApartamentosReservados == null || !command.ApartamentosReservados.Any())
                {
                    return BadRequest(new Response 
                    { 
                        Success = false, 
                        Message = "A reserva deve conter pelo menos um apartamento.",
                        Data = null 
                    });
                }

                var result = await Mediator.Send(command);
                return Ok(new Response 
                { 
                    Success = true, 
                    Message = "Reserva atualizada com sucesso.",
                    Data = result 
                });
            }
            catch (Exception ex) when (ex.Message.Contains("Apartamento") && (ex.Message.Contains("não está disponível") || ex.Message.Contains("está ocupado") || ex.Message.Contains("já está reservado") || ex.Message.Contains("não foi encontrado")))
            {
                // ✅ Erro específico de disponibilidade de apartamento
                return BadRequest(new Response 
                { 
                    Success = false, 
                    Message = "Apartamento não disponível",
                    Data = ex.Message 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new Response 
                { 
                    Success = false, 
                    Message = $"Erro ao processar a atualização da reserva: {ex.Message}",
                    Data = null 
                });
            }
        }        /// <summary>
        /// Verifica disponibilidade de um apartamento em período específico
        /// </summary>
        /// <param name="apartamentoId">ID do apartamento</param>
        /// <param name="dataEntrada">Data de entrada</param>
        /// <param name="dataSaida">Data de saída</param>
        /// <returns>Resultado da verificação</returns>
        [HttpGet("verificar-disponibilidade")]
        public async Task<ActionResult<object>> VerificarDisponibilidade(
            [FromQuery] int apartamentoId,
            [FromQuery] DateTime dataEntrada,
            [FromQuery] DateTime dataSaida)
        {
            try
            {
                // ✅ Usar o novo método implementado que retorna mensagens detalhadas
                var unitOfWork = HttpContext.RequestServices.GetService(typeof(IUnitOfWork)) as IUnitOfWork;
                
                if (unitOfWork == null)
                {
                    return StatusCode(500, new { message = "Serviço não disponível" });
                }

                var (isDisponivel, mensagemErro) = await unitOfWork.Reservas.VerificarDisponibilidadeAsync(apartamentoId, dataEntrada, dataSaida);
                
                return Ok(new 
                { 
                    apartamentoId, 
                    dataEntrada = dataEntrada.ToString("yyyy-MM-dd"), 
                    dataSaida = dataSaida.ToString("yyyy-MM-dd"), 
                    disponivel = isDisponivel,
                    message = isDisponivel ? "Apartamento disponível" : mensagemErro,
                    detalhes = isDisponivel ? null : mensagemErro
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { 
                    message = "Erro ao verificar disponibilidade",
                    erro = ex.Message 
                });
            }
        }

        /// <summary>
        /// Inativa uma reserva específica
        /// </summary>
        /// <param name="id">ID da reserva a ser inativada</param>
        /// <param name="motivoInativacao">Motivo da inativação (opcional)</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("inativar/{id}")]
        public async Task<ActionResult<BaseCommandResponse>> InactivateReserva(
            int id,
            [FromBody] string motivoInativacao = null)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new BaseCommandResponse
                    {
                        Success = false,
                        Message = "❌ ID da reserva deve ser maior que zero",
                        Errors = new List<string> { "ID inválido fornecido" }
                    });
                }

                var command = new InactivateReservaCommand
                {
                    ReservaId = id,
                    MotivoInativacao = motivoInativacao
                };

                var result = await Mediator.Send(command);
                
                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseCommandResponse
                {
                    Success = false,
                    Message = "❌ Erro interno do servidor ao inativar reserva",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
