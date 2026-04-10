
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Hotel.Api.Controllers.Shared
{
    [ApiController]
    //  [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
         protected readonly IUnitOfWork _unitOfWork;
        private ISender _mediator;

        protected ApiControllerBase(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>();
        [HttpPost("test")]
        public async Task<ActionResult> ResponseAsync(BaseCommandResponse response)
        {
            if (!response.Errors.Any())
            {
                try
                {
                    await _unitOfWork.Save();

                    return Ok(response);
                }
                catch (Exception ex)
                {
                    // Aqui devo logar o erro
                    return BadRequest($"Houve um problema interno com o servidor. Entre em contato com o Administrador do sistema caso o problema persista. Erro interno: {ex.Message}");
                    //return Request.CreateResponse(HttpStatusCode.Conflict, $"Houve um problema interno com o servidor. Entre em contato com o Administrador do sistema caso o problema persista. Erro interno: {ex.Message}");
                }
            }
            else
            {
                return Ok(response);
            }
        }

        /*  public Task<IActionResult> ResponseExceptionAsync(Exception ex)
         {
             return Task.FromResult<IActionResult>(BadRequest(new { errors = ex.Message, exception = ex.ToString() }));
             //return Request.CreateResponse(HttpStatusCode.InternalServerError, new { errors = ex.Message, exception = ex.ToString() });
         }
  */
        /*   protected override void Dispose(bool disposing)
          {
              //Realiza o dispose no serviço para que possa ser zerada as notificações
              //if (_serviceBase != null)
              //{
              //    _serviceBase.Dispose();
              //}

              base.Dispose(disposing);
          } */
    }
}