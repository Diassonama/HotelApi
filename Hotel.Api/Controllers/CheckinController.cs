using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Checkin.Commands;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CheckinController : ApiControllerBase
    {
        public CheckinController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        [HttpPost("checkout")]
        public async Task<ActionResult> Post([FromBody] CheckOutCommand checkoutCommand)
        {
            var resposta = await Mediator.Send(checkoutCommand, CancellationToken.None);
            return Ok(resposta);     //await ResponseAsync((BaseCommandResponse)resposta);
        }

        // Hotel.Api/Controllers/CheckinController.cs - apenas o endpoint checkout-manual
        [HttpPost("checkout-manual")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<BaseCommandResponse>> CheckoutManual([FromBody] CheckoutManualCommand command)
        {
            try
            {

                var result = await Mediator.Send(command);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new BaseCommandResponse
                {
                    Success = false,
                    Message = "Erro interno no checkout manual",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
        [HttpGet("{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Checkins>> GetByIdAsync(int id)
        {
            var checkin = await _unitOfWork.checkins.GetByIdAsync(id);

            if (checkin == null)
                return NotFound(new { message = $"Check-in {id} não encontrado." });

            return Ok(checkin);
        }
    }
}