using System.Collections.Generic;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface.Shared;

namespace Hotel.Domain.Interface
{
    public interface IPedidoRepository : IRepositoryBase<Pedido>
    {
        Task<Pedido> GetByIdWithItemsAsync(int id);
        Task<IEnumerable<Pedido>> GetByCheckinIdAsync(int checkinId);
    }
}