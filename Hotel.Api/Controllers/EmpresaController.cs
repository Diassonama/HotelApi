using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Common.PagedResult;
using Hotel.Application.Empresa.Commands;
using Hotel.Application.Empresa.Queries;
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
    public class EmpresaController : ApiControllerBase
    {
        public EmpresaController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        [HttpGet]
        public async Task<BaseCommandResponse> Get()
        {
            return await Mediator.Send(new GetEmpresaQuery());
        }
        [HttpGet("{id}")]
        public async Task<BaseCommandResponse> Get(int id)
        {
            return await Mediator.Send(new GetEmpresaByIdQuery { Id = id });
        }
        [HttpGet("get-with-pagination")]
        public async Task<ActionResult<PagedList<Empresa>>> GetAllFiltro([FromQuery] GetFilteredQuery query)
        {
            return await Mediator.Send(query);
        }
        [HttpPost]
        public async Task<BaseCommandResponse> Post([FromBody] CreateEmpresaCommand createEmpresaCommand)
        {
            return await Mediator.Send(createEmpresaCommand, CancellationToken.None);
        }

        [HttpPut]
        public async Task<BaseCommandResponse> Put([FromBody] UpdateEmpresaCommand updateEmpresaCommand)
        {
            return await Mediator.Send(updateEmpresaCommand, CancellationToken.None);
        }
    }
}