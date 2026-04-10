using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Common.PagedResult;
using Hotel.Application.Paises.Queries;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaisesController : ApiControllerBase
    {
        public PaisesController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
        public async Task<BaseCommandResponse> get(){
            return await Mediator.Send(new GetPaisQuery());
        }
                [HttpGet("get-with-pagination")]
         public async Task<ActionResult<PagedList<Pais>>> GetAllFiltro([FromQuery] GetFilteredPaisQuery query)
        {
            return await Mediator.Send(query);
        }
    }
}