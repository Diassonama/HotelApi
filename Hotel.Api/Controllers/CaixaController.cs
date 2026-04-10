using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Caixa.Commands;
using Hotel.Application.Caixa.Queries;
using Hotel.Application.Common.PagedResult;
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
    public class CaixaController : ApiControllerBase
    {
        public CaixaController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet("get")]
        public async Task<BaseCommandResponse> GetAll()
        {
            return await Mediator.Send(new GetCaixaQuery()); ;
        }

        /*        
        [HttpGet("get-with-pagination")]
         public async Task<ActionResult<PagedList<Caixa>>> GetAllFiltro([FromQuery] GetFilteredCaixaQuery query)
        {
            return await Mediator.Send(query);
        } 
        */

        [HttpGet("{id}")]
        public async Task<ActionResult> get(int id)
        {
            var query = await Mediator.Send(new GetCaixaByIdQuery { Id = id });
            return Ok(query);
        }

        [HttpGet("CaixaAtual")]
        public async Task<int> CaixaAtual()
        {
            return await Mediator.Send(new GetCaixaAtualQuery());
        }

        [HttpGet("MovimentoCaixa")]
        public async Task<object> CaixaMov()
        {
            return await Mediator.Send(new GetMovimentosCaixaQuery());
        }
        
        [HttpPost("{id}/entrada")]
        public async Task<ActionResult> AdicionarEntrada(int id, float valor)
        {
            var query = await Mediator.Send(new AdicionarEntradaCommand { Id = id, Valor = valor });
            return Ok(query);
        }

        [HttpPost("CaixabyDate")]
        public async Task<ActionResult> CaixaBayDate(DateTime data)
        {
            var query = await Mediator.Send(new GetCaixaByDateQuery { data = data });
            return Ok(query);
        }

        [HttpPost("{id}/saida")]
        public async Task<ActionResult> AdicionarSaida(int id, float valor)
        {
            var query = await Mediator.Send(new AdicionarSaidaCommand { Id = id, Valor = valor });
            return Ok(query);
        }

        [HttpPost("{id}/fechar")]
        public async Task<ActionResult> FecharCaixa(int id)
        {
            var query = await Mediator.Send(new FecharCaixaCommand { Id = id });
            return Ok(query);
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Post([FromBody] CreateCaixaCommand createHospedagemCommand)
        {
            var resposta = await Mediator.Send(createHospedagemCommand, CancellationToken.None);
            return Ok(resposta);     //await ResponseAsync((BaseCommandResponse)resposta);
        }
    }
}