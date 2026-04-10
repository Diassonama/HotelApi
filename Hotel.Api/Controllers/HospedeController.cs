using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Hospedes.Commands;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HospedeController : ApiControllerBase
    {
        public HospedeController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
         [HttpPost("Add")]
        public async Task<IActionResult> Post( [FromBody] CreateHospedeCommand createHospedeCommand)
         {
            var resposta = await Mediator.Send(createHospedeCommand, CancellationToken.None);
            return  Ok(resposta);     //await ResponseAsync((BaseCommandResponse)resposta);
        }   
    }
}