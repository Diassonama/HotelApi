using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Api.Extensions;
using Hotel.Application.Common.PagedResult;
using Hotel.Application.Hospedagem.Commands;
using Hotel.Application.Hospedagem.Queries;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HospedagensController : ApiControllerBase
    {
        public HospedagensController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        [HttpGet]
        public async Task<BaseCommandResponse> GetAll()
        {
            return await Mediator.Send(new GetHospedagemQuery()); ;
        }
        [HttpGet("get-with-pagination")]
        public async Task<ActionResult<PagedList<Hospedagem>>> GetAllFiltro([FromQuery] GetFilteredHospedagemQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> get(int id)
        {

            var query = await Mediator.Send(new GetHospedagemByIdQuery { Id = id });
            return Ok(query);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateHospedagemCommand createHospedagemCommand)
        {
            var bb = User.GetUserId();
            var authHeader = Request.Headers["Authorization"];
            Console.WriteLine($"Authorization Header: {authHeader}");
            if (authHeader.ToString().StartsWith("Bearer "))
            {
                var token = authHeader.ToString().Substring("Bearer ".Length).Trim();
                Console.WriteLine($"Token recebido: {token}");
            }

            var resposta = await Mediator.Send(createHospedagemCommand, CancellationToken.None);
            return Ok(resposta);     //await ResponseAsync((BaseCommandResponse)resposta);
        }
        [HttpPut]
        public Task<BaseCommandResponse> update([FromBody] UpdateHospedagemCommand updateHospedagem)
        {
            return Mediator.Send(updateHospedagem);
        }
        
        [HttpPut("transferencia")]
[Authorize]
public async Task<ActionResult<BaseCommandResponse>> TransferirHospedagem([FromBody] TransferenciaHospedagemCommand command)
{
    try
    {
       // command.Id = id;
        var resultado = await Mediator.Send(command);
        
        if (resultado.Success)
        {
            return Ok(resultado);
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