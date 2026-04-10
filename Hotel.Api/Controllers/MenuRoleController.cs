using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.MenuRole.Base;
using Hotel.Application.MenuRole.Commands;
using Hotel.Application.MenuRole.Queries;
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
    public class MenuRoleController : ApiControllerBase
    {
        private readonly IMenuRoleRepository _menuRoleRepository;
        public MenuRoleController(IUnitOfWork unitOfWork, IMenuRoleRepository menuRoleRepository) : base(unitOfWork)
        {
            _menuRoleRepository = menuRoleRepository;
        }

        [HttpGet("GetMenuRoleByRole")]
        public async Task<BaseCommandResponse> GetMenuRoleByRole(string perfil)
        {
            return await Mediator.Send(new GetMenuRoleByIdQuery { Id = perfil });
        }

        [HttpGet]
        public async Task<BaseCommandResponse> Get()
        {
            return await Mediator.Send(new GetMenuRoleQuery());
        }

        [HttpPost]
        public async Task<IActionResult> post(MenuRole[] menu)
        {
            return Ok(await _menuRoleRepository.InsertMenuAsync(menu));
        }
    }
}



/*
public async Task<List<BaseCommandResponse>> Add([FromBody] CreateMenuRoleCommand[] createMenuRoleCommand)
{
    var responses = new List<BaseCommandResponse>();
    /* 
                foreach (var command in createMenuRoleCommand)
                {
                    var response = await Mediator.Send(command);
                        responses.Add(response); 
     */

/* foreach (var vap in command.command)
{
    var response = await Mediator.Send(command);
    responses.Add(response);
} */

//  }
// var response = await Mediator.Send(createMenuRoleCommand);

/*
             foreach (var commandWrapper in createMenuRoleCommand[0].command)
            {
               // foreach (var item in commandWrapper.)
               // {
                    // Cria um novo comando para cada item no array `command`
                    var individualCommand = new MenuRoleCommandBase
                    {
                        MenuId = commandWrapper.MenuId,
                        RoleId = commandWrapper.RoleId
                    };

                    // Envia o comando individual para o Mediator
                    var response = await Mediator.Send(individualCommand);

                    // Adiciona a resposta à lista
                    responses.Add(response);
              //  }
            } 

            return responses;
        }
    }
}*/
