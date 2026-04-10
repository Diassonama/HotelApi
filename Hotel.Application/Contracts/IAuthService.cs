using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common;
using Hotel.Application.DTOs.Request;
using Hotel.Application.DTOs.Response;
using Hotel.Application.Responses;
using Hotel.Domain.Identity;

namespace Hotel.Application.Contracts
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(UsuarioLoginRequest request);
        Task<RegistrationResponse> Register(RegistrationRequest request);
        Task<UsuarioCadastroResponse> CadastrarUsuario(UsuarioCadastroRequest usuarioCadastro);
        Task<UsuarioLoginResponse> Login_Alter(UsuarioLoginRequest usuarioLogin);
        Task<UsuarioLoginResponse> LoginSemSenha(string usuarioId);
        Task<ApplicationUser> Usuariologado(string usuarioId);

        Task<string> GetUsernameByIdAsync(string userId);
        Task<string> GetUsernameByEmailAsync(string email);
        Task<string> GetUserEmailByIdAsync(string id);
        Task<ApplicationUser> FindUserByEmailAsync(string email);
        Task<string> GetUserPhoneByIdAsync(string id);

        Task<bool> IsEmailConfirmed(string userId);
        Task<bool> IsPhoneConfirmed(string phone);

        Task<string> GetUsernameByUsernameAsync(string username);
        Task<string> GetUserIdByUsernameAsync(string username);
        Task<string> GetUserIdByEmailAsync(string email);
        Task<ApplicationUser> FindUserByIdAsync(string userId);
        Task<DateTime?> GetUserLastLoginDateAsync(string username);
        
        Task<bool> VerifyTokenAsync(string email, string token);
        Task<bool> VerifyPhoneTokenAsync(int phone, string token);
        Task<bool> ConfirmEmailAsync(string token, string email);
        Task<bool> ConfirmPhoneAsync(string token, string userId, string phoneNumber);
        
        Task<string> GeneratePasswordResetTokenAsync(string username);
        Task<bool> ResetPasswordAsync(string username, string token, string newPassword);
        Task<bool> VerifyPasswordResetTokenAsync(string username, string token);
        Task<string> GenerateEmailConfirmationTokenAsync(string email);
        Task<string> GeneratePhoneConfirmationTokenAsync(string userId, string phone);
        Task<string> GenerateEmailChangeTokenAsync(string userId, string newEmail);
        Task<string> GeneratePhoneChangeTokenAsync(string userId, string newPhoneNumber);
        Task<string> GeneratePhoneChangeTokenAsyncUser(string username, string newPhoneNumber);

        Task<BaseCommandResponse> DeleteUserAsync(string userId);
        Task<BaseCommandResponse> UpdateUserAsync(UsuarioUpdateRequest request);
        Task<Result> ChangeUserPasswordAsync(string userId, string currentPassword, string newPassword);

        Task<BaseCommandResponse> UpdateUserEmailAsync(string userId, string email);
        Task<BaseCommandResponse> UpdateUserLastLoginAsync(string userId);
        Task<BaseCommandResponse> UpdateUserEmailByTokenAsync(string userId, string email, string token);

        Task<(BaseCommandResponse, string)> SignInAsync(string originKeyValue, string username, string password, bool rememberMe, string ipAddress, string province, bool mustConfirm = false, string push_token = "");
        Task<BaseCommandResponse> SignOutAsync(string userId);
    }
}
