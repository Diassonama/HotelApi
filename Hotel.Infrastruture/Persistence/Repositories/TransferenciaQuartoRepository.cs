using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class TransferenciaQuartoRepository : RepositoryBase<TransferenciaQuarto>, ITransferenciaQuartoRepository
    {
        private readonly GhotelDbContext _context;

        public TransferenciaQuartoRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransferenciaQuarto>> GetByCheckinIdAsync(int checkinId)
        {
            return await _context.TransferenciaQuartos
                .Include(t => t.Checkins)
                .Include(t => t.Apartamentos)
                 //   .ThenInclude(h => h.Apartam)
              //  .Include(t => t.HospedagemDestino)
               //     .ThenInclude(h => h.Apartamentos)
                .Include(t => t.MotivoTransferencia)
                .Where(t => t.CheckinId == checkinId)
                .OrderByDescending(t => t.DataTransferencia)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransferenciaQuarto>> GetByHospedagemOrigemIdAsync(int hospedagemId)
        {
            return await _context.TransferenciaQuartos
                .Include(t => t.Checkins)
               /*  .Include(t => t.HospedagemOrigem)
                    .ThenInclude(h => h.Apartamentos)
                .Include(t => t.HospedagemDestino)
                    .ThenInclude(h => h.Apartamentos) */
                .Include(t => t.MotivoTransferencia)
                .Where(t => t.CheckinId == hospedagemId)
                .OrderByDescending(t => t.DataTransferencia)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransferenciaQuarto>> GetByHospedagemDestinoIdAsync(int hospedagemId)
        {
            return await _context.TransferenciaQuartos
                .Include(t => t.Checkins).ThenInclude(c => c.Hospedagem)
                .Include(t => t.Apartamentos)
                  //  .Include(h => h.ApartamentoDestino)
                //  .Include(t => t.HospedagemDestino)
                //     .ThenInclude(h => h.Apartamentos)
                .Include(t => t.MotivoTransferencia)
             //   .Where(t => t.HospedagemDestinoId == hospedagemId)
                 .Where(t => t.CheckinId == hospedagemId)
                .OrderByDescending(t => t.DataTransferencia)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransferenciaQuarto>> GetByMotivoIdAsync(int motivoId)
        {
            return await _context.TransferenciaQuartos
                .Include(t => t.Checkins)
               /*  .Include(t => t.HospedagemOrigem)
                    .ThenInclude(h => h.Apartamentos)
                .Include(t => t.HospedagemDestino)
                    .ThenInclude(h => h.Apartamentos) */
                .Include(t => t.MotivoTransferencia)
                .Where(t => t.MotivoTransferenciaId == motivoId)
                .OrderByDescending(t => t.DataTransferencia)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransferenciaQuarto>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.TransferenciaQuartos
                .Include(t => t.Checkins)
               /*  .Include(t => t.HospedagemOrigem)
                    .ThenInclude(h => h.Apartamentos)
                .Include(t => t.HospedagemDestino)
                    .ThenInclude(h => h.Apartamentos) */
                .Include(t => t.MotivoTransferencia)
                .Where(t => t.DataTransferencia >= dataInicio && t.DataTransferencia <= dataFim)
                .OrderByDescending(t => t.DataTransferencia)
                .ToListAsync();
        }

        public  async Task<TransferenciaQuarto> GetByIdAsync(int id)
        {
            return await _context.TransferenciaQuartos
                .Include(t => t.Checkins)
                .Include(t => t.MotivoTransferencia)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public  async Task<IEnumerable<TransferenciaQuarto>> GetAllAsync()
        {
            return await _context.TransferenciaQuartos
                .Include(t => t.Checkins)
                .Include(t => t.MotivoTransferencia)
                .OrderByDescending(t => t.DataTransferencia)
                .ToListAsync();
        }
    }
}