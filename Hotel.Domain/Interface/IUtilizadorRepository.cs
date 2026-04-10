
using Hotel.Domain.Dtos;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;


namespace Hotel.Domain.Interface
{
    public interface IUtilizadorRepository: IRepositoryBase<Utilizador>
    {
        Task<IEnumerable<Utilizador>> usuarios();
        Task<IEnumerable<UsuariosResponse>> UsuariosComRoles();
        Task<UsuariosResponse> GetByIdAsync(string id);
        Task<string> GetNomeCompletoByIdAsync(string id);
    }
}