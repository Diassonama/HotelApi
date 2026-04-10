using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Common.PagedResult;
using Hotel.Application.DTOs.TipoApartamento;
using Hotel.Application.Menu.Queries;
using Hotel.Application.Responses;
using Hotel.Application.TipoApartamento.Commands;
using Hotel.Application.TipoApartamento.Queries;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TiposApartamentosController : ApiControllerBase
    {
        public TiposApartamentosController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        [HttpGet]
        public async Task<BaseCommandResponse> GetAll()
        {
            return await Mediator.Send(new GetAllTipoApartamentoQuery()); ;
        }
        [HttpGet("get-with-pagination")]
        public async Task<ActionResult<PagedList<TipoApartamento>>> GetAllFiltro([FromQuery] GetFilteredTipoApartamentoQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> get(int id)
        {
            var query = await Mediator.Send(new GetTipoApartamentoByIdQuery { Id = id });
            return Ok(query);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CreateTipoApartamentoCommand createTipoApartamentoCommand)
        {
            var resposta = await Mediator.Send(createTipoApartamentoCommand, CancellationToken.None);
            return Ok(resposta);     //await ResponseAsync((BaseCommandResponse)resposta);
        }

        
        [HttpPut]
        public async Task<ActionResult> Put([FromBody] UpdateTipoApartamentoCommand updateTipoApartamentoCommand)
        {
            var resposta = await Mediator.Send(updateTipoApartamentoCommand, CancellationToken.None);
            return Ok(resposta);     //await ResponseAsync((BaseCommandResponse)resposta);
        }
       
    }
}