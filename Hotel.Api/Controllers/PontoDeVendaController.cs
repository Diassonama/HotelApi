using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PontoDeVendaController : ApiControllerBase
    {
        private readonly ILogger<PontoDeVendaController> _logger;

        public PontoDeVendaController(IUnitOfWork unitOfWork, ILogger<PontoDeVendaController> logger) : base(unitOfWork)
        {
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PontoDeVenda>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var pontos = await _unitOfWork.GetRepository<PontoDeVenda>().GetAllAsync();
                return Ok(pontos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar pontos de venda");
                return StatusCode(500, new { message = "Erro interno ao listar pontos de venda." });
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PontoDeVenda), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var ponto = await _unitOfWork.GetRepository<PontoDeVenda>().Get(id);
                if (ponto == null)
                {
                    return NotFound(new { message = $"Ponto de venda com ID {id} não encontrado." });
                }

                return Ok(ponto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar ponto de venda com ID {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao buscar ponto de venda." });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(PontoDeVenda), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] PontoDeVenda request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Nome))
                {
                    return BadRequest(new { message = "Nome do ponto de venda é obrigatório." });
                }

                var ponto = new PontoDeVenda
                {
                    Nome = request.Nome.Trim()
                };

                await _unitOfWork.GetRepository<PontoDeVenda>().Add(ponto);
                await _unitOfWork.Save();

                return CreatedAtAction(nameof(GetById), new { id = ponto.Id }, ponto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar ponto de venda");
                return StatusCode(500, new { message = "Erro interno ao criar ponto de venda." });
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(PontoDeVenda), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] PontoDeVenda request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Nome))
                {
                    return BadRequest(new { message = "Nome do ponto de venda é obrigatório." });
                }

                var repository = _unitOfWork.GetRepository<PontoDeVenda>();
                var ponto = await repository.Get(id);

                if (ponto == null)
                {
                    return NotFound(new { message = $"Ponto de venda com ID {id} não encontrado." });
                }

                ponto.Nome = request.Nome.Trim();

                await repository.Update(ponto);
                await _unitOfWork.Save();

                return Ok(ponto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar ponto de venda com ID {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao atualizar ponto de venda." });
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var repository = _unitOfWork.GetRepository<PontoDeVenda>();
                var ponto = await repository.Get(id);

                if (ponto == null)
                {
                    return NotFound(new { message = $"Ponto de venda com ID {id} não encontrado." });
                }

                await repository.Delete(id);
                await _unitOfWork.Save();

                return Ok(new { message = "Ponto de venda removido com sucesso." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover ponto de venda com ID {Id}", id);
                return StatusCode(500, new { message = "Erro interno ao remover ponto de venda." });
            }
        }
    }
}