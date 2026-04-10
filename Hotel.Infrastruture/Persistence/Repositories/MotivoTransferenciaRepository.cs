using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class MotivoTransferenciaRepository : RepositoryBase<MotivoTransferencia>, IMotivoTransferenciaRepository
    {
        private readonly GhotelDbContext _context;

        public MotivoTransferenciaRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MotivoTransferencia>> GetAtivosAsync()
        {
            return await _context.MotivoTransferencias
                .Where(m => m.IsActive)
                .OrderBy(m => m.Descricao)
                .ToListAsync();
        }

        public async Task<MotivoTransferencia> GetByCodigoAsync(string descricao)
        {
            return await _context.MotivoTransferencias
                .FirstOrDefaultAsync(m => m.Descricao == descricao && m.IsActive);
        }

        /* public  async Task<IEnumerable<MotivoTransferencia>> GetAllAsync()
        {
            return await _context.MotivoTransferencias
                .OrderBy(m => m.Descricao)
                .ToListAsync();
        }*/
    } 
}