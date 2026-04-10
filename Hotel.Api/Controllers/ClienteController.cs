
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Cliente.Commands;
using Hotel.Application.Cliente.Queries;
using Hotel.Application.Common.PagedResult;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClienteController : ApiControllerBase
    {
        
        public ClienteController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
         public async Task<IEnumerable<Cliente>> GetAllUsers()
        {
            return await Mediator.Send(new GetAllClienteQuery());;
        }
        [HttpGet("get-with-pagination")]
         public async Task<ActionResult<PagedList<Cliente>>> GetAllFiltro([FromQuery] GetFilteredClienteQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpGet("{id}")]
         public async  Task<ActionResult> get( int id)
        {
            var query =  await Mediator.Send(new GetClienteByIdQuery { Id = id });
            return Ok(query);
        } 

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateClienteCommand createClienteCommand)
        {
            var resposta = await Mediator.Send(createClienteCommand, CancellationToken.None);
            return Ok(resposta);
        }

        [HttpPut]
        public async Task<IActionResult> Put( [FromBody] UpdateClienteCommand updateClienteCommand)
        {
            var resposta = await Mediator.Send(updateClienteCommand);
            return Ok(resposta);
        } 
    }
}