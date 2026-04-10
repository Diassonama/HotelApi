using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Hotel.Api.Controllers.Shared;
using Hotel.Application.Common;
using Hotel.Application.Contracts;
using Hotel.Application.DTOs.Request;
using Hotel.Application.DTOs.Response;
using Hotel.Application.Interfaces;
using Hotel.Application.Interfaces.Services;
using Hotel.Application.Responses;
using Hotel.Application.User.Commands.changePassword;
using Hotel.Application.User.Commands.ConfirmEmail;
using Hotel.Application.User.Commands.ConfirmRecoveryCode;
using Hotel.Application.User.Commands.LogoutUser;
using Hotel.Application.User.Commands.RecoverPassword;
using Hotel.Domain.Dtos;
using Hotel.Domain.Entities;
using Hotel.Domain.Identity;







//using Hotel.Application.Interfaces.Services;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ApiControllerBase
    {
        //  private readonly IIdentityService _identityService;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly ISMSService _smsService;
        // private readonly IConfiguration _config;
        private readonly IConfigService _config;
        public readonly GhotelDbContext _context;
        private readonly UsuarioLogado _usuarioLogado;
        private readonly IUtilizadorRepository _repository;
        
        public UsuarioController(IUnitOfWork unitOfWork, IAuthService authService, IEmailService emailService, IConfigService config, ISMSService smsService, GhotelDbContext context, UsuarioLogado usuarioLogado, IUtilizadorRepository repository) : base(unitOfWork)
        {
            _authService = authService;
            _emailService = emailService;
            _config = config;
            _smsService = smsService;
            _context = context;
            _usuarioLogado = usuarioLogado;
            _repository = repository;
        }
        [Authorize]
        [HttpPut("update")]
        public async Task<ActionResult<BaseCommandResponse>> UpdateUsuario([FromBody] UsuarioUpdateRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var resposta = new BaseCommandResponse
                    {
                        Success = false,
                        Message = "❌ Dados inválidos fornecidos",
                        Errors = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToList()
                    };
                    return BadRequest(resposta);
                }

                var resultado = await _authService.UpdateUserAsync(request);
                
                if (resultado.Success)
                {
                    return Ok(resultado);
                }
                else
                {
                    return BadRequest(resultado);
                }
            }
            catch (Exception ex)
            {
                var resposta = new BaseCommandResponse
                {
                    Success = false,
                    Message = "❌ Erro interno do servidor ao atualizar usuário",
                    Errors = new List<string> { ex.Message }
                };
                return StatusCode(500, resposta);
            }
        }
       

        [HttpPost("register")]
        public async Task<IActionResult> Cadastrar(UsuarioCadastroRequest usuarioCadastro)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var resultado = await _authService.CadastrarUsuario(usuarioCadastro);
            if (resultado.Sucesso)
            {
                Log.Information("Usuário registrado com sucesso: {Email}", usuarioCadastro.Email);
 // Enviar SMS de confirmação
                var userId = await _authService.GetUserIdByEmailAsync(usuarioCadastro.Email);         
            var smstoken = await _authService.GeneratePhoneChangeTokenAsync(userId, "922285032");
           await _smsService.SendConfirmationSMS("922285032", smstoken);
            }
           
          
           

            if (resultado.Sucesso)
                return Ok(resultado);
              /*    var userId = await _authService.GetUserIdByEmailAsync(usuarioCadastro.Email);
                var emailToken = await _authService.GenerateEmailConfirmationTokenAsync(usuarioCadastro.Email);
                  var smstoken = await _authService.GeneratePhoneChangeTokenAsync(userId,"922285032");
                  await _smsService.SendConfirmationSMS( "922285032",smstoken) ;
               if (emailToken != null)
               {
                   //  await _emailService.SendEmailAfterRegister(usuarioCadastro.Email, emailToken);
                   //   await _smsService.SendConfirmationSMS( "922285032",smstoken) ;
               }
               else if (resultado.Erros.Count > 0)
               {
                   var problemDetails = new CustomProblemDetails(HttpStatusCode.BadRequest, Request, errors: resultado.Erros);
                   return BadRequest(problemDetails);
               }  */

            return StatusCode(StatusCodes.Status500InternalServerError);
            //    return Ok(await _authService.Register(usuarioCadastro));
        }

        [HttpPost("login")]
        public async Task<ActionResult<UsuarioCadastroResponse>> Login(UsuarioLoginRequest usuarioLogin)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var resultado = await _authService.Login_Alter(usuarioLogin);
            if (resultado.Sucesso)
                return Ok(resultado);

            return Unauthorized();
        }
        /*  [HttpPost("login2")]
         public async Task<ActionResult<AuthResponse>> Login2(UsuarioLoginRequest request)
         {
             return Ok(await _authService.Login(request));
         } */

        //    [Authorize(Policy = "AllowAllUsers")]
        [HttpPost("change-password")]
        public async Task<ActionResult<Result>> ChangePassword(ChangePasswordCommad command)
        {
            return await Mediator.Send(command);
        }

        /*   [HttpPost("register")]
          public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request)
          {
              return Ok(await _authService.Register(request));
          } */

        [Authorize]
        [HttpPost("refresh-login")]
        public async Task<ActionResult<UsuarioCadastroResponse>> RefreshLogin()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var usuarioId = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var flo = identity?.FindFirst(ClaimTypes.UserData)?.Value;
            if (usuarioId == null)
                return BadRequest();

            var resultado = await _authService.LoginSemSenha(usuarioId);
            if (resultado.Sucesso)
                return Ok(resultado);

            return Unauthorized();
        }
        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<ActionResult<HttpResponseMessage>> ConfirmEmail(string email, string token)
        {
            var command = new ConfirmeEmailCommand { Email = email, Token = token };
            await Mediator.Send(command);
            return Redirect(_config.Getvalue("frontOfficeUrl"));
        }

        //[Authorize(Policy = "AllowAllUsers")]
        [HttpPost("logout")]
        public async Task<ActionResult<BaseCommandResponse>> Logout(LogoutUserCommand command)
        {
            return await Mediator.Send(command);
        }
        [AllowAnonymous]
        // [TypeFilter(typeof(RateLimitFilter), Arguments = new object[] { 60 })]
        [HttpPost("recover-password")]
        public async Task<ActionResult<string>> RecoverPassword(RecoverPasswordCommand command)
        {
            return await Mediator.Send(command);
        }

        [AllowAnonymous]
        // [TypeFilter(typeof(RateLimitFilter), Arguments = new object[] { 60 })]
        [HttpPost("confirm-recovery-code")]
        public async Task<ActionResult<BaseCommandResponse>> ConfirmRecoveryCode(ConfirmRecoveryCodeCommand command)
        {
            return await Mediator.Send(command);
        }

        [Authorize]
        [HttpGet("usuariologado")]
        public async Task<ActionResult<BaseCommandResponse>> UsuarioLogado()
        {
            var resposta = new BaseCommandResponse();
            
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                var usuarioId = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(usuarioId))
                {
                    resposta.Success = false;
                    resposta.Message = "❌ Usuário não autenticado";
                    return Unauthorized(resposta);
                }

                var utilizador = await _authService.Usuariologado(_usuarioLogado.IdUtilizador);

                if (utilizador == null)
                {
                    resposta.Success = false;
                    resposta.Message = "❌ Usuário não encontrado";
                    return NotFound(resposta);
                }

                var logado = new UsuarioLogadoRequest()
                {
                    Email = utilizador.Email,
                    FirstName = utilizador.FirstName,
                    Id = utilizador.Id,
                    LastName = utilizador.LastName,
                    UserName = utilizador.UserName
                 //   Roles = await _authService.GetUserRolesAsync(utilizador)
                };

                resposta.Success = true;
                resposta.Message = "✅ Dados do usuário logado obtidos com sucesso";
                resposta.Data = logado;
                
                return Ok(resposta);
            }
            catch (Exception ex)
            {
                resposta.Success = false;
                resposta.Message = "❌ Erro interno do servidor ao obter dados do usuário logado";
                resposta.Errors = new List<string> { ex.Message };
                return StatusCode(500, resposta);
            }
        }
        [Authorize]
        [HttpGet("getAll")]
        public async Task<ActionResult<BaseCommandResponse>> GetAllUsers()
        {
            var resposta = new BaseCommandResponse();
            
            try
            {
                // Usar o novo método que retorna usuários com roles
                var usuarios = await _repository.UsuariosComRoles();
                                            
                if (usuarios == null || !usuarios.Any())
                {
                    resposta.Success = false;
                    resposta.Message = "❌ Nenhum usuário encontrado";
                    return Ok(resposta);
                }

                resposta.Success = true;
                resposta.Message = "✅ Usuários carregados com sucesso";
                resposta.Data = usuarios;
                
                return Ok(resposta);
            }
            catch (Exception ex)
            {
                resposta.Success = false;
                resposta.Message = "❌ Erro interno do servidor ao buscar usuários";
                resposta.Errors = new List<string> { ex.Message };
                return StatusCode(500, resposta);
            }
        }

        [Authorize]
        [HttpGet("GetUsr")]
        public ActionResult<BaseCommandResponse> GetUserInfo()
        {
            var resposta = new BaseCommandResponse();
            
            try
            {
                var userName = User.Identity.Name; // Nome do usuário
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // ID do usuário
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value; // Email do usuário

                if (string.IsNullOrEmpty(userId))
                {
                    resposta.Success = false;
                    resposta.Message = "❌ Usuário não autenticado";
                    return Unauthorized(resposta);
                }

                resposta.Success = true;
                resposta.Message = "✅ Informações do usuário obtidas com sucesso";
                resposta.Data = new 
                { 
                    UserName = userName, 
                    UserId = userId,
                    Email = userEmail
                };
                
                return Ok(resposta);
            }
            catch (Exception ex)
            {
                resposta.Success = false;
                resposta.Message = "❌ Erro interno do servidor ao obter informações do usuário";
                resposta.Errors = new List<string> { ex.Message };
                return StatusCode(500, resposta);
            }
        }

        [Authorize]
        [HttpGet("getAllWithRoles")]
        public async Task<ActionResult<BaseCommandResponse>> GetAllUsersWithRoles()
        {
            var resposta = new BaseCommandResponse();
            
            try
            {
                // Usar o método que funciona com JOIN
                var usuarios = await _repository.UsuariosComRoles();
                                            
                if (usuarios == null || !usuarios.Any())
                {
                    resposta.Success = false;
                    resposta.Message = "❌ Nenhum usuário encontrado";
                    return Ok(resposta);
                }

                resposta.Success = true;
                resposta.Message = $"✅ {usuarios.Count()} usuários carregados com roles";
                resposta.Data = usuarios;
                
                return Ok(resposta);
            }
            catch (Exception ex)
            {
                resposta.Success = false;
                resposta.Message = "❌ Erro interno do servidor ao buscar usuários com roles";
                resposta.Errors = new List<string> { ex.Message };
                return StatusCode(500, resposta);
            }
        }
        
        /*  [AllowAnonymous]
        // [TypeFilter(typeof(RateLimitFilter), Arguments = new object[] { 30 })]
         [HttpPost("reset-password")]
         public async Task<bool> ResetPassword(ResetPasswordCommand command)
         {
             return await Mediator.Send(command);
         } */

    }
}