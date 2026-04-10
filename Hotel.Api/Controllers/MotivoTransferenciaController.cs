
using Hotel.Api.Controllers.Shared;
using Hotel.Application.MotivoTransferencia.Commands;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{

   
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MotivoTransferenciaController : ApiControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;

        public MotivoTransferenciaController(IUnitOfWork unitOfWork): base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        /// <summary>
        /// Obtém todos os motivos de transferência
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Domain.Entities.MotivoTransferencia>>> GetAll()
        {
            try
            {
                var motivos = await _unitOfWork.MotivoTransferencia.GetAllAsync();
                return Ok(motivos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obtém motivos de transferência ativos
        /// </summary>
        [HttpGet("ativos")]
        public async Task<ActionResult<IEnumerable<Domain.Entities.MotivoTransferencia>>> GetAtivos()
        {
            try
            {
                var motivos = await _unitOfWork.MotivoTransferencia.GetAtivosAsync();
                return Ok(motivos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Obtém um motivo de transferência por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Domain.Entities.MotivoTransferencia>> GetById(string descricao)
        {
            try
            {
                var motivo = await _unitOfWork.MotivoTransferencia.GetByCodigoAsync(descricao);
                
                if (motivo == null)
                {
                    return NotFound(new { message = "Motivo de transferência não encontrado" });
                }

                return Ok(motivo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro interno: {ex.Message}" });
            }
        }

        /// <summary>
        /// Cria um novo motivo de transferência
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BaseCommandResponse>> Create([FromBody] CreateMotivoTransferenciaCommand command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var resultado = await Mediator.Send(command);
                
                if (resultado.Success)
                {
                    return CreatedAtAction(nameof(GetById), new { id = ((Domain.Entities.MotivoTransferencia)resultado.Data).Id }, resultado);
                }
                
                return BadRequest(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseCommandResponse
                {
                    Success = false,
                    Message = $"Erro interno: {ex.Message}"
                });
            }
        }
    }
}