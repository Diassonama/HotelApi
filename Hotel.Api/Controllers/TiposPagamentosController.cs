using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Responses;
using Hotel.Application.TipoPagamentos.Queries;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TiposPagamentosController : ApiControllerBase
    {
        public TiposPagamentosController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
[HttpGet]
[Authorize]
        public Task<BaseCommandResponse> Get()
        {
            return  Mediator.Send(new GetTipoPagamentosQuery());          
        }
    }
}