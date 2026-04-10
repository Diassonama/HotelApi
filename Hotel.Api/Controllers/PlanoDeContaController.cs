using Hotel.Api.Controllers.Shared;
using Hotel.Application.PlanoDeConta.Commands;
using Hotel.Application.PlanoDeConta.Queries;
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
    public class PlanoDeContaController : ApiControllerBase
    {
        
        public PlanoDeContaController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
         public async Task<IEnumerable<PlanoDeConta>> GetAllPlanoDeContas()
        {
            return await Mediator.Send(new GetAllPlanoDeContaQuery());;
        }
        
      /*   [HttpGet("get-with-pagination")]
         public async Task<ActionResult<PagedList<PlanoDeConta>>> GetAllFiltro([FromQuery] GetFilteredPlanoDeContaQuery query)
        {
            return await Mediator.Send(query);
        } */

        [HttpGet("{id}")]
         public async  Task<ActionResult> get( int id)
        {
            var query =  await Mediator.Send(new GetPlanoDeContaByIdQuery { Id = id });
            return Ok(query);
        } 

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreatePlanoDeContaCommand createPlanoDeContaCommand)
        {
            var resposta = await Mediator.Send(createPlanoDeContaCommand, CancellationToken.None);
            return Ok(resposta);
        }

        [HttpPut]
        public async Task<IActionResult> Put( [FromBody] UpdatePlanoDeContaCommand updatePlanoDeContaCommand)
        {
            var resposta = await Mediator.Send(updatePlanoDeContaCommand);
            return Ok(resposta);
        } 

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var resposta = await Mediator.Send(new DeletePlanoDeContaCommand { Id = id });
            return Ok(resposta);
        }
    }
}
