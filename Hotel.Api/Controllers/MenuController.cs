using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Apartamentos.Queries;
using Hotel.Application.Common.PagedResult;
using Hotel.Application.DTOs.Request;
using Hotel.Application.Hospedagem.Queries;
using Hotel.Application.Menu.Commands;
using Hotel.Application.Menu.Queries;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MenuController : ApiControllerBase
    {
        public MenuController(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        [HttpGet]
         public async Task<BaseCommandResponse> GetAll()
        {
            return await Mediator.Send(new GetMenuQuery()); ;
        }
         [HttpGet("get-with-pagination")]
         public async Task<ActionResult<PagedList<AppMenu>>> GetAllFiltro([FromQuery] GetMenuFilterQuery query)
        {
            return await Mediator.Send(query);
        }
          [HttpGet("get-with-paginationGen")]
         public async Task<ActionResult<PagedList<AppMenu>>> GetAllFiltroGen([FromQuery] GetFilteredGenericoQuery query)
        {
            return await Mediator.Send(query);
        }
        [HttpGet("{id}")]
         public async Task<BaseCommandResponse> Get(int id)
        {
            return await Mediator.Send(new GetMenuByIdQuery {Id = id}); 
        }
        [HttpGet("MenuByRole")]
         public async Task<BaseCommandResponse> Role(string perfil)
        {
            return await Mediator.Send(new GetMenuByRoleQuery {Id = perfil}); 
        }
        
        [HttpGet("HaveAccess")]
         public async Task<BaseCommandResponse> RoleAcess(string path, string roleName)
        {
            return await Mediator.Send(new HaveAcessoQuery { Path = path, RoleName = roleName}); 
        }
        [HttpGet("Accesso")]
         public async Task<BaseCommandResponse> Acesso(string id)
        {
            return await Mediator.Send(new GetMenuAcessoByRoleQuery { Id = id}); 
        }

        [HttpPut("Update")]
         public async Task<BaseCommandResponse> update([FromBody] UpdateMenuCommand updateMenuCommand )
        {
            return await Mediator.Send(updateMenuCommand ); 
        }
        
        [HttpPut("{id}")]
         public async Task<BaseCommandResponse> UpdateMenu(int id, [FromBody] UpdateMenuCommand updateMenuCommand )
        {
            updateMenuCommand.Id = id;
            return await Mediator.Send(updateMenuCommand ); 
        }
        
        [HttpPost("Add")]
         public async Task<BaseCommandResponse> Add([FromBody] CreateMenuCommand createMenuCommand )
        {
            return await Mediator.Send(createMenuCommand ); 
        }

        [HttpPost]
         public async Task<BaseCommandResponse> CreateMenu([FromBody] CreateMenuCommand createMenuCommand )
        {
            return await Mediator.Send(createMenuCommand ); 
        }

    }
}