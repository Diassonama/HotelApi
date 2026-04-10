using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.DTOs.Request;
using Hotel.Application.DTOs.Response;

namespace Hotel.Application.Contracts
{
    public interface IIdentityService
    {
        Task<UsuarioCadastroResponse> CadastrarUsuario(UsuarioCadastroRequest usuarioCadastro);
        Task<UsuarioLoginResponse> Login(UsuarioLoginRequest usuarioLogin);
        Task<UsuarioLoginResponse> LoginSemSenha(string usuarioId);
    }
}