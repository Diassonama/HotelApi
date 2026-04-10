using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.ConfiguracaoFiscal.Commands;
using Hotel.Application.ConfiguracaoFiscal.Queries;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
  //  [Authorize]
    public class ConfiguracaoFiscalController : ApiControllerBase
    {
        public ConfiguracaoFiscalController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
        [HttpGet]
        public async Task<BaseCommandResponse> Get()
        {
           return await Mediator.Send(new GetAllConfiguracaoFiscalQuery());
        }

        [HttpGet("{id}")]
        public async Task<BaseCommandResponse> Get(int id)
        {
            return await Mediator.Send(new GetConfiguracaoFiscalByIdQuery {Id = id});
        }
         [HttpGet("AppConfigAll")]  
        public async Task<BaseCommandResponse> GetAppConfig()
        {
            return await Mediator.Send(new GetAppConfigAllQuery());
        }
        [HttpGet("keyvalues")]
    public async Task<IActionResult> Getkey() => Ok(await Mediator.Send(new GetKeyValuesQuery()));


        [HttpGet("AppConfig")]  
        public async Task<BaseCommandResponse> GetApp(string key)
        {
            return await Mediator.Send(new GetAppConfigQuery {key = key});
        }



        // POST api/values  
        [HttpPost]  
        public async Task<BaseCommandResponse> Post([FromBody] CreateConfiguracaoFiscalCommand createConfiguracaoFiscalCommand)
        {
            return await Mediator.Send(createConfiguracaoFiscalCommand);
        }
        // PUT api/values/5
        [HttpPut("Update")]
        public async Task<BaseCommandResponse> Put([FromBody] UpdateConfiguracaoFiscalCommand updateConfiguracaoFiscalCommand)  
        {
            return await Mediator.Send(updateConfiguracaoFiscalCommand);
        }
    }
}