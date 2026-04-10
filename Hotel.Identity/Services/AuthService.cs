using Hotel.Application.Common;
using Hotel.Application.Constants;
using Hotel.Application.Contracts;
using Hotel.Application.DTOs.Request;
using Hotel.Application.DTOs.Response;
using Hotel.Application.Interfaces;
using Hotel.Application.Responses;
using Hotel.Domain.Enums;
using Hotel.Domain.Identity;
using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Web;
using Hotel.Infrastruture.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Hotel.Domain.Dtos;


namespace Hotel.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSettings _jwtSettings;
        private readonly ITokenService _tokenService;
        private readonly IClaimService _claimService;
        private readonly IEmailService _emailService;
        private readonly ILogger _logger;

        public AuthService(UserManager<ApplicationUser> userManager,
                IOptions<JwtSettings> jwtSettings,
                SignInManager<ApplicationUser> signInManager,
                RoleManager<IdentityRole> roleManager,
                ITokenService tokenService,
                IClaimService claimService,
                IEmailService emailService,
                ILogger logger)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _claimService = claimService;
            _emailService = emailService;
            _logger = logger;
            // Url = urlHelper;
        }
        public async Task<AuthResponse> Login(UsuarioLoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                throw new Exception($"User with {request.Email} not found.");
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Senha, false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                throw new Exception($"Credentials for '{request.Email} aren't valid'.");
            }

            JwtSecurityToken jwtSecurityToken = await GenerateToken(user);

            AuthResponse response = new AuthResponse
            {
                Id = user.Id.ToString(),
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = user.Email,
                UserName = user.UserName
            };

            return response;
        }
        public async Task<RegistrationResponse> Register(RegistrationRequest request)
        {
            // await CreateRolesandUsers();
            var existingUser = await _userManager.FindByNameAsync(request.UserName);

            if (existingUser != null)
            {
                throw new Exception($"Username '{request.UserName}' already exists.");
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName,
                EmailConfirmed = true
            };
            
            var existingEmail = await _userManager.FindByEmailAsync(request.Email);

            if (existingEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Hotel");
                    return new RegistrationResponse() { UserId = user.Id.ToString() };
                }
                else
                {
                    throw new Exception($"{result.Errors}");
                }
            }
            else
            {
                throw new Exception($"Email {request.Email} already exists.");
            }
        }


        public async Task<BaseCommandResponse> UpdateUserAsync(UsuarioUpdateRequest request)
        {
            var response = new BaseCommandResponse();
            
            try
            {
                // Buscar usuário existente por ID
                var existingUser = await _userManager.FindByIdAsync(request.Id);
                
                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = $"❌ Usuário com ID '{request.Id}' não encontrado.";
                    return response;
                }

                // Verificar se email já existe para outro usuário
                if (!string.IsNullOrEmpty(request.Email) && request.Email != existingUser.Email)
                {
                    var emailExists = await _userManager.FindByEmailAsync(request.Email);
                    if (emailExists != null && emailExists.Id != existingUser.Id)
                    {
                        response.Success = false;
                        response.Message = $"❌ Email '{request.Email}' já está sendo usado por outro usuário.";
                        return response;
                    }
                }

                // Atualizar propriedades do usuário
                existingUser.Email = request.Email ?? existingUser.Email;
                existingUser.UserName = request.Email ?? existingUser.UserName; // Usar email como username
                existingUser.FirstName = request.PrimeiroNome ?? existingUser.FirstName;
                existingUser.LastName = request.UltimoNome ?? existingUser.LastName;

                // Atualizar usuário
                var updateResult = await _userManager.UpdateAsync(existingUser);
                
                if (!updateResult.Succeeded)
                {
                    response.Success = false;
                    response.Message = "❌ Erro ao atualizar dados do usuário.";
                    response.Errors = updateResult.Errors.Select(e => e.Description).ToList();
                    return response;
                }

                // Atualizar senha se fornecida
                if (!string.IsNullOrEmpty(request.Senha))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                    var passwordResult = await _userManager.ResetPasswordAsync(existingUser, token, request.Senha);
                    
                    if (!passwordResult.Succeeded)
                    {
                        response.Success = false;
                        response.Message = "❌ Erro ao atualizar senha do usuário.";
                        response.Errors = passwordResult.Errors.Select(e => e.Description).ToList();
                        return response;
                    }
                }

                // Atualizar roles se necessário
                 if (request.Perfil != default)
                {
                    var currentRoles = await _userManager.GetRolesAsync(existingUser);
                    var newRole = request.Perfil.ToString();
                    
                    // Remover roles atuais se diferentes
                    if (currentRoles.Any() && !currentRoles.Contains(newRole))
                    {
                        await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                    }
                    
                    // Adicionar novo role se não existir
                    if (!currentRoles.Contains(newRole))
                    {
                        await _userManager.AddToRoleAsync(existingUser, newRole);
                    }
                } 

                response.Success = true;
                response.Message = "✅ Usuário atualizado com sucesso.";
                response.Data = new 
                { 
                    Id = existingUser.Id,
                    Email = existingUser.Email,
                    UserName = existingUser.UserName,
                    FirstName = existingUser.FirstName,
                    LastName = existingUser.LastName
                };
                
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error($"Erro ao atualizar usuário {request.Id}: {ex}");
                response.Success = false;
                response.Message = "❌ Erro interno do servidor ao atualizar usuário.";
                response.Errors = new List<string> { ex.Message };
                return response;
            }
        }

        private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(CustomClaimTypes.Uid, user.Id.ToString())
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);
            return jwtSecurityToken;
        }
        // }


        /*  public async Task<UsuarioCadastroResponse> CadastrarUsuario(UsuarioCadastroRequest usuarioCadastro)
         {
             //  await CreateRolesandUsers();

              if (usuarioCadastro == null)
         {
             throw new ArgumentNullException(nameof(usuarioCadastro), "Dados de cadastro não podem ser nulos");
         }


         Log.Information("Iniciando cadastro de usuário: {Email}", usuarioCadastro.Email);       
         if (string.IsNullOrEmpty(usuarioCadastro.Email))
         {
             throw new ArgumentException("Email é obrigatório", nameof(usuarioCadastro.Email));
         }

             var existingUser = await _userManager.FindByNameAsync(usuarioCadastro.Email);
 Log.Information("Verificando existência do usuário: {Email}", usuarioCadastro.Email);
             if (existingUser != null)
             {
                 throw new Exception($" Usuario '{usuarioCadastro.Email}' ja existe.");
             }
             Log.Information("Criando usuário: {Email}", usuarioCadastro.Email);
             var ApplicationUser = new ApplicationUser
             {
                 Id = Guid.NewGuid().ToString(),
                 UserName = usuarioCadastro.Email,
                 Email = usuarioCadastro.Email,
                 FirstName = usuarioCadastro.PrimeiroNome,
                 LastName = usuarioCadastro.UltimoNome,
                 EmailConfirmed = false,
                 SecurityStamp = Guid.NewGuid().ToString(),
                 ConcurrencyStamp = Guid.NewGuid().ToString()
             };
             Log.Information("Usuário criado: {Email}", usuarioCadastro.Email);
             // creating Creating Manager role     
             var existingEmail = await _userManager.FindByEmailAsync(usuarioCadastro.Email);
             //If user has e-mail confirm it
             //   var successConfirmEmail = false;
             if (existingEmail == null)
             {
                 Log.Information("Verificando existência do email: {Email}", usuarioCadastro.Email);
                 var result = await _userManager.CreateAsync(ApplicationUser, usuarioCadastro.Senha);
                 if (result.Succeeded)
                     Log.Information("Usuário criado com sucesso: {Email}", usuarioCadastro.Email);
                     await _userManager.SetLockoutEnabledAsync(ApplicationUser, true);

                 var roleName = usuarioCadastro.Perfil.ToString();
                 if (!await _roleManager.RoleExistsAsync(roleName))
                 {
                     var role = new IdentityRole(roleName);
                     await _roleManager.CreateAsync(role);
                 }

                 await _userManager.AddToRoleAsync(ApplicationUser, roleName);
                 Log.Information("Adicionando usuário à role: {Role}", roleName);
                 var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(ApplicationUser);
                 var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));
                 //var confirmationLink = Url.Action("ConfirmEmail", "Account", new { token, email = usuarioCadastro.Email }, "https");
                 Log.Information("Enviando email de confirmação para: {Email}", usuarioCadastro.Email);
                 await _emailService.SendEmailAfterRegister(usuarioCadastro.Email, encodedToken);
                 Log.Information("Email de confirmação enviado para: {Email}", usuarioCadastro.Email);
                 var usuarioCadastroResponse = new UsuarioCadastroResponse(result.Succeeded);
                 if (!result.Succeeded && result.Errors.Count() > 0)
                     usuarioCadastroResponse.AdicionarErros(result.Errors.Select(r => r.Description));
                 Log.Information("Cadastro de usuário concluído: {Email}", usuarioCadastro.Email);
                 return usuarioCadastroResponse;

             }
             else
             {
                 throw new Exception($"Email {usuarioCadastro.Email} ja existe.");
             }
         }

         */
       
       
public async Task<UsuarioCadastroResponse> CadastrarUsuario(UsuarioCadastroRequest usuarioCadastro)
{
   
    
    try
                {
                    // ✅ VALIDAÇÃO 1: Request nulo
                    if (usuarioCadastro == null)
                    {
                        Log.Error("UsuarioCadastroRequest é nulo");
                        var errorResponse = new UsuarioCadastroResponse(false);
                        errorResponse.AdicionarErros(new[] { "Dados de cadastro não podem ser nulos" });
                        return errorResponse;
                    }

                    // ✅ VALIDAÇÃO 2: Email obrigatório
                    if (string.IsNullOrWhiteSpace(usuarioCadastro.Email))
                    {
                        Log.Warning("Tentativa de cadastro sem email válido");
                        var errorResponse = new UsuarioCadastroResponse(false);
                        errorResponse.AdicionarErros(new[] { "Email é obrigatório" }); // ✅ CORRIGIDO
                        return errorResponse;
                    }

                    // ✅ VALIDAÇÃO 3: Senha obrigatória
                    if (string.IsNullOrWhiteSpace(usuarioCadastro.Senha))
                    {
                        Log.Warning("Tentativa de cadastro sem senha para email: {Email}", usuarioCadastro.Email);
                        var errorResponse = new UsuarioCadastroResponse(false);
                        errorResponse.AdicionarErros(new[] { "Senha é obrigatória" }); // ✅ CORRIGIDO
                        return errorResponse;
                    }

                    // ✅ VALIDAÇÃO 4: Nome obrigatório
                    if (string.IsNullOrWhiteSpace(usuarioCadastro.PrimeiroNome))
                    {
                        Log.Warning("Tentativa de cadastro sem nome para email: {Email}", usuarioCadastro.Email);
                        var errorResponse = new UsuarioCadastroResponse(false);
                        errorResponse.AdicionarErros(new[] { "Nome é obrigatório" }); // ✅ CORRIGIDO

                        return errorResponse;
                    }

                    Log.Information("Iniciando cadastro de usuário: {Email}, Nome: {Nome}",
                        usuarioCadastro.Email, usuarioCadastro.PrimeiroNome);

                    // ✅ VERIFICAÇÃO 1: Usuário já existe por UserName
                    var existingUserByName = await _userManager.FindByNameAsync(usuarioCadastro.Email);
                    if (existingUserByName != null)
                    {
                        Log.Warning("Tentativa de cadastro com username já existente: {Email}", usuarioCadastro.Email);
                        var errorResponse = new UsuarioCadastroResponse(false);
                        errorResponse.AdicionarErros(new[] { $"Usuário '{usuarioCadastro.Email}' já existe" }); // ✅ CORRIGIDO
                        return errorResponse;
                    }

                    // ✅ VERIFICAÇÃO 2: Email já existe
                    var existingUserByEmail = await _userManager.FindByEmailAsync(usuarioCadastro.Email);
                    if (existingUserByEmail != null)
                    {
                        Log.Warning("Tentativa de cadastro com email já existente: {Email}", usuarioCadastro.Email);
                        var errorResponse = new UsuarioCadastroResponse(false);
                        errorResponse.AdicionarErros(new[] { $"Email '{usuarioCadastro.Email}' já está em uso" }); // ✅ CORRIGIDO
                        return errorResponse;
                    }

                    Log.Information("Validações concluídas. Criando ApplicationUser para: {Email}", usuarioCadastro.Email);

                    // ✅ CRIAÇÃO DO USUÁRIO: Todos os campos obrigatórios
                    var newUser = new ApplicationUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserName = usuarioCadastro.Email?.Trim(),
                        Email = usuarioCadastro.Email?.Trim(),
                        FirstName = usuarioCadastro.PrimeiroNome?.Trim(),
                        LastName = usuarioCadastro.UltimoNome?.Trim(),
                        EmailConfirmed = false,
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled = false,
                        LockoutEnabled = true,
                        AccessFailedCount = 0,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                        NormalizedEmail = usuarioCadastro.Email?.Trim().ToUpper(),
                        NormalizedUserName = usuarioCadastro.Email?.Trim().ToUpper()
                    };

                    Log.Information("ApplicationUser criado: {UserId}, Email: {Email}, Nome: {Nome} {Sobrenome}",
                        newUser.Id, newUser.Email, newUser.FirstName, newUser.LastName);

                    // ✅ CRIAÇÃO NO BANCO: UserManager.CreateAsync
                    var createResult = await _userManager.CreateAsync(newUser, usuarioCadastro.Senha);

                    if (!createResult.Succeeded)
                    {
                        var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                        Log.Error("Falha na criação do usuário {Email}. Erros: {Errors}", usuarioCadastro.Email, errors);

                        var errorResponse = new UsuarioCadastroResponse(false);
                        errorResponse.AdicionarErros(createResult.Errors.Select(e => e.Description));
                        return errorResponse;
                    }

                    Log.Information("Usuário criado com sucesso no banco: {Email}, UserId: {UserId}",
                        usuarioCadastro.Email, newUser.Id);

                    // ✅ CONFIGURAÇÕES PÓS-CRIAÇÃO
                    try
                    {
                        // Habilitar lockout
                        await _userManager.SetLockoutEnabledAsync(newUser, true);
                        Log.Information("Lockout habilitado para usuário: {UserId}", newUser.Id);

                        // ✅ GERENCIAMENTO DE ROLES
                        var roleName = usuarioCadastro.Perfil.ToString();

                        // Verificar se role existe, criar se necessário
                        if (!await _roleManager.RoleExistsAsync(roleName))
                        {
                            Log.Information("Criando role '{Role}' para usuário: {Email}", roleName, usuarioCadastro.Email);
                            var role = new IdentityRole(roleName)
                            {
                                NormalizedName = roleName.ToUpper()
                            };
                            var roleResult = await _roleManager.CreateAsync(role);

                            if (!roleResult.Succeeded)
                            {
                                var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                                Log.Warning("Erro ao criar role '{Role}': {Errors}", roleName, roleErrors);
                            }
                        }

                        // Adicionar usuário à role
                        var addToRoleResult = await _userManager.AddToRoleAsync(newUser, roleName);
                        if (addToRoleResult.Succeeded)
                        {
                            Log.Information("Usuário {Email} adicionado à role '{Role}' com sucesso", usuarioCadastro.Email, roleName);
                        }
                        else
                        {
                            var roleErrors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
                            Log.Warning("Erro ao adicionar usuário {Email} à role '{Role}': {Errors}",
                                usuarioCadastro.Email, roleName, roleErrors);
                        }

                        // ✅ ENVIO DE EMAIL DE CONFIRMAÇÃO
                        try
                        {
                            Log.Information("Gerando token de confirmação de email para: {Email}", usuarioCadastro.Email);

                            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailConfirmationToken));

                            Log.Information("Token gerado. Enviando email de confirmação para: {Email}", usuarioCadastro.Email);

                            // Verificar se serviço de email está disponível
                            if (_emailService != null)
                            {
                                await _emailService.SendEmailAfterRegister(usuarioCadastro.Email, encodedToken);
                                Log.Information("Email de confirmação enviado com sucesso para: {Email}", usuarioCadastro.Email);
                            }
                            else
                            {
                                Log.Warning("Serviço de email não disponível. Token de confirmação gerado: {Email}", usuarioCadastro.Email);
                            }
                        }
                        catch (Exception emailEx)
                        {
                            // Não falhar o cadastro por erro de email
                            Log.Error(emailEx, "Erro ao enviar email de confirmação para {Email}. Cadastro mantido.", usuarioCadastro.Email);
                        }

                    }
                    catch (Exception postCreationEx)
                    {
                        Log.Error(postCreationEx, "Erro nas configurações pós-criação para usuário {Email}. Usuário foi criado mas pode ter configurações incompletas.", usuarioCadastro.Email);
                        // Continuar - usuário foi criado com sucesso
                    }

                    // ✅ RESPOSTA DE SUCESSO
                    var successResponse = new UsuarioCadastroResponse(true);
                    successResponse.UsuarioId = newUser.Id; // ✅ IMPORTANTE: Definir o ID do usuário criado

                    Log.Information("=== CADASTRO CONCLUÍDO COM SUCESSO ===");
                    Log.Information("Usuário cadastrado: Email={Email}, UserId={UserId}, Nome={Nome}",
                        usuarioCadastro.Email, newUser.Id, usuarioCadastro.PrimeiroNome);

                    return successResponse;
                }
                catch (Exception ex)
                {
                    // ✅ TRATAMENTO DE ERRO GLOBAL
                    Log.Error(ex, "=== ERRO NO CADASTRO DE USUÁRIO ===");
                    Log.Error("Email: {Email}, Erro: {Error}", usuarioCadastro?.Email ?? "N/A", ex.Message);

                    var errorResponse = new UsuarioCadastroResponse(false);

                    // Personalizar mensagem de erro baseada no tipo de exceção
                    if (ex is ArgumentException || ex is ArgumentNullException)
                    {
                        errorResponse.AdicionarErros(new[] { ex.Message }); // ✅ CORRIGIDO
                    }
                    else if (ex.Message.Contains("já existe") || ex.Message.Contains("already exists"))
                    {
                        errorResponse.AdicionarErros(new[] { "Email ou usuário já cadastrado no sistema" });
                    }
                    else
                    {
                        errorResponse.AdicionarErros(new[] { "Erro interno do servidor. Tente novamente em alguns minutos." }); // ✅ CORRIGIDO

                        // Log detalhado para debugging
                        Log.Error("Stack trace: {StackTrace}", ex.StackTrace);
                    }

                    return errorResponse;
                }
}
       


       
        public async Task SendeEmail(ApplicationUser applicationUser, UsuarioCadastroRequest usuarioCadastro)
        {
            // var successConfirmEmail = false;
            var emailConfirmationTokenGenerated = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
            var emailConfirmationTokenEncoded = HttpUtility.UrlEncode(emailConfirmationTokenGenerated);
            // successConfirmEmail = await ConfirmEmailAsync(emailConfirmationTokenEncoded, usuarioCadastro.Email);
            await _emailService.SendEmailAfterRegister(usuarioCadastro.Email, emailConfirmationTokenEncoded);

        }

        private async Task CreateRolesandUsers()
        {
            bool x = await _roleManager.RoleExistsAsync("SuperAdmin");
            if (!x)
            {
                var user = new ApplicationUser();
                user.Id = "b609ca9d-79a6-416c-84d6-33b1c382686a"; //Guid.NewGuid().ToString();
                user.UserName = "SuperAdmin";
                user.Email = "superAdmin@superAdmin.com";
                string userPWD = "P@ssw0rd2024";

                IdentityResult chkUser = await _userManager.CreateAsync(user, userPWD);

                //Add default User to Role Admin    
                if (chkUser.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "SuperAdmin");
                }
            }

            /*           // creating Creating Hotel role     
                      x = await _roleManager.RoleExistsAsync("Hotel");
                      if (!x)
                      {
                          var role = new IdentityRole();
                          role.Name = "Hotel";
                          await _roleManager.CreateAsync(role);
                      }

                      // creating Creating Finanças role     
                      x = await _roleManager.RoleExistsAsync("Finanças");
                      if (!x)
                      {
                          var role = new IdentityRole();
                          role.Name = "Finanças";
                          await _roleManager.CreateAsync(role);
                      }
                      x = await _roleManager.RoleExistsAsync("Lavandaria");
                      if (!x)
                      {
                          var role = new IdentityRole();
                          role.Name = Roles.Lavanderia.ToString();
                          await _roleManager.CreateAsync(role);
                      }
                      x = await _roleManager.RoleExistsAsync("Restaurante");
                      if (!x)
                      {
                          var role = new IdentityRole();
                          role.Name = Roles.Restaurante.ToString();
                          await _roleManager.CreateAsync(role);
                      } */
        }

        public async Task<UsuarioLoginResponse> Login_Alter(UsuarioLoginRequest usuarioLogin)
        {
            var result = await _signInManager.PasswordSignInAsync(usuarioLogin.Email, usuarioLogin.Senha, false, false);

            if (result.Succeeded)
            {
                // ✅ CORREÇÃO: Usar FindByNameAsync em vez de SingleOrDefault para evitar conflitos de DbContext
                var user = await _userManager.FindByNameAsync(usuarioLogin.Email);
                if (user == null) throw new Exception("User not found");
                
                var token = await _tokenService.GerarCredenciais(usuarioLogin.Email);
                await _userManager.SetAuthenticationTokenAsync(user, "Default", "authToken", token.AccessToken);
                user.RefreshToken = _tokenService.GenerateRefreshToken(token.RefreshToken);
                await _userManager.UpdateAsync(user);
                return token;
            }

            var usuarioLoginResponse = new UsuarioLoginResponse();
            if (!result.Succeeded)
            {
                if (result.IsLockedOut)
                    usuarioLoginResponse.AdicionarErro("Essa conta está bloqueada");
                else if (result.IsNotAllowed)
                    usuarioLoginResponse.AdicionarErro("Essa conta não tem permissão para fazer login");
                else if (result.RequiresTwoFactor)
                    usuarioLoginResponse.AdicionarErro("É necessário confirmar o login no seu segundo fator de autenticação");
                else
                    usuarioLoginResponse.AdicionarErro("Usuário ou senha estão incorretos");
            }

            return usuarioLoginResponse;
        }

        public async Task<UsuarioLoginResponse> LoginSemSenha(string usuarioId)
        {
            var usuarioLoginResponse = new UsuarioLoginResponse();
            var usuario = await _userManager.FindByIdAsync(usuarioId);

            if (await _userManager.IsLockedOutAsync(usuario))
                usuarioLoginResponse.AdicionarErro("Essa conta está bloqueada");
            else if (!await _userManager.IsEmailConfirmedAsync(usuario))
                usuarioLoginResponse.AdicionarErro("Essa conta precisa confirmar seu e-mail antes de realizar o login");

            if (usuarioLoginResponse.Sucesso)
                return await _tokenService.GerarCredenciais(usuario.Email);

            return usuarioLoginResponse;
        }

        public Task<bool> VerifyTokenAsync(string email, string token)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerifyPhoneTokenAsync(int phone, string token)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ConfirmEmailAsync(string token, string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return false;

                var user = await _userManager.FindByEmailAsync(email);

                if (user is null)
                {
                    return false;
                }

                var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
                var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
                _logger.Information($"Confirmação do Email {JsonSerializer.Serialize(result)}");
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.Information(string.Format("IdentityService - ConfirmEmailAsync - Error confirm email. Exception: {0}", ex));
                throw;
            }
        }

        public Task<bool> ConfirmPhoneAsync(string token, string userId, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GeneratePasswordResetTokenAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                throw new Exception($"Usuario {username} não encontrado.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;       //Ok(new { Token = token });
        }

        public async Task<bool> ResetPasswordAsync(string username, string token, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    _logger.Warning($"Usuário não encontrado para reset de senha: {username}");
                    return false;
                }

                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (result.Succeeded)
                {
                    _logger.Information($"Senha resetada com sucesso para usuário: {username}");
                    return true;
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.Warning($"Erro ao resetar senha para {username}: {errors}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Exceção ao resetar senha para {username}: {ex}");
                return false;
            }
        }

        public async Task<bool> VerifyPasswordResetTokenAsync(string username, string token)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    _logger.Warning($"Usuário não encontrado para verificação de token: {username}");
                    return false;
                }

                var isValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.Error($"Exceção ao verificar token para {username}: {ex}");
                return false;
            }
        }

        public Task<string> GenerateEmailConfirmationTokenAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GeneratePhoneConfirmationTokenAsync(string userId, string phone)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.GenerateChangePhoneNumberTokenAsync(user, phone);
        }

        public Task<string> GenerateEmailChangeTokenAsync(string userId, string newEmail)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GeneratePhoneChangeTokenAsync(string userId, string newPhoneNumber)
        {
            // ✅ CORREÇÃO: Verificar se userId não é nulo ou vazio
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("ID do usuário não pode ser nulo ou vazio", nameof(userId));
            }

            var user = await _userManager.FindByIdAsync(userId);
            
            // ✅ CORREÇÃO: Verificar se o usuário foi encontrado
            if (user == null)
            {
                throw new Exception($"Usuário com ID '{userId}' não encontrado.");
            }

            // ✅ CORREÇÃO: Verificar se newPhoneNumber não é nulo
            if (string.IsNullOrEmpty(newPhoneNumber))
            {
                throw new ArgumentException("Novo número de telefone não pode ser nulo ou vazio", nameof(newPhoneNumber));
            }

            return await _userManager.GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber);
        }

        public async Task<string> GeneratePhoneChangeTokenAsyncUser(string userName, string newPhoneNumber)
        {
            // ✅ CORREÇÃO: Verificar se userName não é nulo ou vazio
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Nome de usuário não pode ser nulo ou vazio", nameof(userName));
            }

            // ✅ CORREÇÃO: Usar FindByNameAsync em vez de SingleOrDefault para evitar conflitos de DbContext
            var user = await _userManager.FindByNameAsync(userName);
            
            // ✅ CORREÇÃO: Verificar se o usuário foi encontrado
            if (user == null)
            {
                throw new Exception($"Usuário '{userName}' não encontrado.");
            }

            // ✅ CORREÇÃO: Verificar se newPhoneNumber não é nulo
            if (string.IsNullOrEmpty(newPhoneNumber))
            {
                throw new ArgumentException("Novo número de telefone não pode ser nulo ou vazio", nameof(newPhoneNumber));
            }
            
            return await _userManager.GenerateChangePhoneNumberTokenAsync(user, newPhoneNumber);
        }
        public Task<BaseCommandResponse> DeleteUserAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> ChangeUserPasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);


            if (user != null)
            {
                return await ChangeUserPasswordAsync(user, currentPassword, newPassword);
            }

            return Result.Success();
        }
        public async Task<Result> ChangeUserPasswordAsync(ApplicationUser user, string currentPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            return result.ToApplicationResult();
        }

        public async Task<ApplicationUser> Usuariologado(string usuarioId)
        {
            var resposta = new BaseCommandResponse();
            var user = await _userManager.FindByIdAsync(usuarioId);

            /*  if(user ==null){
                 resposta.Message ="Usuario não esta logado";
                 resposta.Success = false;
                 return resposta;
             }

             resposta.Message = "Usuario logado";
             resposta.Data = user;
             resposta.Success= true; */

            return user;
        }
  /*       public async Task<UsuariosResponse> usuarios()
        {
            var utilizadoresComRoles = await _context.Utilizadores
     .Include(u => u.UtilizadorRoles)
         .ThenInclude(ur => ur.RoleId)
     .Select(u => new UsuariosResponse
     {
         Id = u.Id,
         Email = u.Email,
         UserName = u.UserName,
         FirstName = u.FirstName,
         LastName = u.LastName,
         UserRoles = u.UtilizadorRoles.Select(userRole => new RoleResponse
         {
             Id = userRole.UserId,
             Name = userRole.RoleId

         }).ToList()
         // Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
     })
     .ToListAsync();


            var resultado = utilizadoresComRoles;
            return resultado;
        } */
        public Task<BaseCommandResponse> UpdateUserEmailAsync(string userId, string email)
        {
            throw new NotImplementedException();
        }

        public Task<BaseCommandResponse> UpdateUserLastLoginAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseCommandResponse> UpdateUserEmailByTokenAsync(string userId, string email, string token)
        {
            throw new NotImplementedException();
        }

        public Task<(BaseCommandResponse, string)> SignInAsync(string originKeyValue, string username, string password, bool rememberMe, string ipAddress, string province, bool mustConfirm = false, string push_token = "")
        {
            throw new NotImplementedException();
        }

        public async Task<BaseCommandResponse> SignOutAsync(string userId)
        {
            await _signInManager.SignOutAsync();

            var user = _userManager.Users.Include(t => t.RefreshToken).SingleOrDefault(u => u.Id == userId);

            if (user == null)
                throw new Exception("User not found");

            await _userManager.RemoveAuthenticationTokenAsync(user, "Default", "authToken");
            if (user.RefreshToken != null)
                user.RefreshToken.Revoked = DateTime.Now;
            await _userManager.UpdateAsync(user);

            BaseCommandResponse response = new BaseCommandResponse();
            response.Success = true;

            return response;//        BaseCommandResponse.Success();
        }

        public async Task<string> GetUsernameByIdAsync(string userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);

            return user?.UserName;
        }
        public async Task<ApplicationUser> FindUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return user;
        }
        public async Task<ApplicationUser> FindUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user;
        }


        public async Task<string> GetUsernameByEmailAsync(string email)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);

            return user?.UserName;
        }

        public async Task<string> GetUserEmailByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            return user?.Email;
        }

        public async Task<string> GetUserPhoneByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            return user?.PhoneNumber;
        }

        public async Task<bool> IsEmailConfirmed(string userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return (user == null) || user.EmailConfirmed || string.IsNullOrEmpty(user.Email);
        }

        public async Task<bool> IsPhoneConfirmed(string userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            return (user == null) || user.PhoneNumberConfirmed;
        }

        public async Task<string> GetUsernameByUsernameAsync(string username)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
            //var claims = await _userManager.GetClaimsAsync(user);
            return user?.UserName;
        }

        public async Task<string> GetUserIdByUsernameAsync(string username)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == username);
            return user?.Id;
        }
        public async Task<string> GetUserIdByEmailAsync(string email)
        {
            // var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
            var user = await _userManager.FindByEmailAsync(email);
            Log.Information($"GetUserIdByEmailAsync - Email: {email}, UserId: {user?.Id}");
            return user?.Id;
        }

        public Task<DateTime?> GetUserLastLoginDateAsync(string username)
        {
            throw new NotImplementedException();
        }



        /*    private async Task<UsuarioLoginResponse> GerarCredenciais(string email)
          {
             var claims = new List<Claim>();

             claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()));
             // claims.Add(new Claim(JwtRegisteredClaimNames.NameIdentifier, user.Id));
             claims.Add(new Claim(JwtRegisteredClaimNames.Name, user.UserName));
             claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
             claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
             claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()));
             claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()));

             if (adicionarClaimsUsuario)
             {
                 var userClaims = await _userManager.GetClaimsAsync(user);
                 var roles = await _userManager.GetRolesAsync(user);

                 claims.AddRange(userClaims);

                 foreach (var role in roles)
                     claims.Add(new Claim("role", role));
             }

             return claims;
         }  */
    }
}