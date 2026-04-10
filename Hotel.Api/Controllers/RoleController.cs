using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Responses;
using Hotel.Application.Roles.Commands;
using Hotel.Application.Roles.Queries;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoleController : ApiControllerBase
    {
        private readonly GhotelDbContext _context;
        public RoleController(IUnitOfWork unitOfWork, GhotelDbContext context) : base(unitOfWork)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<BaseCommandResponse> GetAll()
        {
            var resposta = new BaseCommandResponse();
           var lista = await _context.Roles.ToListAsync();
               // .Select(r => new { r.Id, r.Name })
             

            if(lista ==null)
            {
                resposta.Success= false;
                resposta.Message = "Dados não encontrados";
               return resposta;
            }

            resposta.Data = lista;
            resposta.Message = "Dados Carregados com sucesso";
            resposta.Success = true;
            return resposta;


           // return await Mediator.Send(new GetAllRoleQuery()); ;
        }
        [HttpGet("{id}")]
        public async Task<BaseCommandResponse> Get(string id)
        {
            return await Mediator.Send(new GetRoleByIdQuery { Id = id });
        }

        [HttpPost("Add")]
        public async Task<BaseCommandResponse> Add([FromBody] CreateRoleCommand createRoleCommand)
        {
            return await Mediator.Send(createRoleCommand);
        }

        /// <summary>
        /// Atualiza um perfil/role existente
        /// </summary>
        /// <param name="updateRoleCommand">Dados do perfil a ser atualizado</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("Update")]
        public async Task<BaseCommandResponse> Update([FromBody] UpdateRoleCommand updateRoleCommand)
        {
            return await Mediator.Send(updateRoleCommand);
        }

        /// <summary>
        /// Atualiza um perfil/role existente (endpoint REST padrão)
        /// </summary>
        /// <param name="id">ID do perfil</param>
        /// <param name="updateRoleCommand">Dados do perfil a ser atualizado</param>
        /// <returns>Resultado da operação</returns>
        [HttpPut("{id}")]
        public async Task<BaseCommandResponse> UpdateRole(string id, [FromBody] UpdateRoleCommand updateRoleCommand)
        {
            updateRoleCommand.Id = id;
            return await Mediator.Send(updateRoleCommand);
        }

        /// <summary>
        /// Cria um novo perfil/role (endpoint REST padrão)
        /// </summary>
        /// <param name="createRoleCommand">Dados do perfil a ser criado</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost]
        public async Task<BaseCommandResponse> CreateRole([FromBody] CreateRoleCommand createRoleCommand)
        {
            return await Mediator.Send(createRoleCommand);
        }
    }
}