
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Mvc;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Apartamentos.Queries;
using Hotel.Application.Apartamentos.Commands;
using Hotel.Application.Apartamento.Commands;
using Hotel.Application.Common.PagedResult;
using Hotel.Application.Apartamento.Queries;
using Microsoft.AspNetCore.Authorization;
using Hotel.Application.Responses;
using Hotel.Domain.Enums;



namespace Hotel.Api.Controllers
{
    //[ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ApartamentoController : ApiControllerBase
    {
        public ApartamentoController(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        [HttpGet("get")]
        public async Task<IEnumerable<Apartamentos>> GetAllUsers()
        {
            return await Mediator.Send(new GetAllApartamentoQuery()); ;
        }

         [HttpGet("ocupados")]
        public async Task<IEnumerable<Apartamentos>> GetAPartamentoOcupados()
        {
            return await Mediator.Send(new GetApartamentoOcupadosQuery()); ;
        }

        [HttpGet("filtroSituacao")]
        public async Task<BaseCommandResponse> GetfiltroSituacao(Situacao situacao)
        {
            return await Mediator.Send(new GetApartamentoFiltroSituacaoQuery { Situacao = situacao }); ;
        }
        [HttpGet("QuartoStatus")]
        public async Task<BaseCommandResponse> Get()
        {
            return await Mediator.Send(new GetApartamentoPorStatusQuery()); ;
        }
        [HttpGet("QuartoAtrasados")]
        public async Task<BaseCommandResponse> GetAtrasados()
        {
            return await Mediator.Send(new GetApartamentosAtrasadoQuery()); ;
        }
        [HttpGet("get-with-pagination")]
        public async Task<ActionResult<PagedList<Apartamentos>>> GetAllFiltro([FromQuery] GetFilteredAparamentoQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> get(int id)
        {
            var query = await Mediator.Send(new GetApartamentoByIdQuery { Id = id });
            return Ok(query);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Post([FromBody] CreateApartamentoCommand createApartamentoCommand)
        {
            var resposta = await Mediator.Send(createApartamentoCommand, CancellationToken.None);
            return Ok(resposta);     //await ResponseAsync((BaseCommandResponse)resposta);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Put([FromBody] UpdateApartamentoCommand updateApartamentoCommand)
        {
            var resposta = await Mediator.Send(updateApartamentoCommand);
            return Ok(resposta);
        }

        [HttpGet("livres")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Apartamentos>>> GetApartamentosLivres()
        {
            try
            {
                var apartamentosLivres = await _unitOfWork.Apartamento.GetBySituacaoAsync(Situacao.Livre);

                var resultado = new
                {
                    Total = apartamentosLivres.Count,
                    Situacao = "Livre",
                    DataConsulta = DateTime.UtcNow,
                    Apartamentos = apartamentosLivres.Select(a => new
                    {
                        Id = a.Id,
                        Codigo = a.Codigo,

                        Situacao = a.Situacao.ToString(),
                        TipoApartamento = a.TipoApartamentos?.Descricao ?? "N/A",
                        CheckinsId = a.CheckinsId,

                    }).ToList()
                };

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = $"Erro ao buscar apartamentos livres: {ex.Message}",
                    timestamp = DateTime.UtcNow
                });
            }
        }
       
    }
}
