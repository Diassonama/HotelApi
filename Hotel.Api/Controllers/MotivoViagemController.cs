using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.MotivoViagem.Queries;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MotivoViagemController : ApiControllerBase
    {
        public MotivoViagemController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        [HttpGet]
        public async Task<BaseCommandResponse> Get()
        {
            return await Mediator.Send(new GetMotivoViagemQuery()); ;
        }
    }
}