using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IMotivoTransferenciaRepository : IRepositoryBase<MotivoTransferencia>
    {
        Task<IEnumerable<MotivoTransferencia>> GetAtivosAsync();


        Task<MotivoTransferencia> GetByCodigoAsync(string descricao);
    }
}