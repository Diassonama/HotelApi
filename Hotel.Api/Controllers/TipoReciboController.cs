using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using Hotel.Application.TipoRecibo.Queries;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoReciboController : ApiControllerBase
    {


        public TipoReciboController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
         public async Task<BaseCommandResponse> Get()
        {
            return await Mediator.Send(new GetTipoRecibosQuery());
        }
        //using Hotel.Api.Models;
        

    }
}