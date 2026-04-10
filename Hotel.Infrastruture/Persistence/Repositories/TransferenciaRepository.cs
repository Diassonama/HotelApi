using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class TransferenciaRepository : RepositoryBase<Transferencia>, ITransferenciaRepository
    {
        private readonly GhotelDbContext _context;

        public TransferenciaRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }
         public async Task<IEnumerable<Transferencia>> GetByCheckinIdAsync(int checkinId)
        {
            return await _context.Transferencias
                .Where(t => t.CheckinId == checkinId )
                        .OrderBy(t => t.DataEntrada)
                        .AsNoTracking()
                        .ToListAsync();
        }
 public async Task<int> GetCountAsync()
        {
            return await _context.Transferencias
                .AsNoTracking()
                .CountAsync();
        }

         public async Task<int> GetCountByCheckinIdAsync(int checkinId)
        {
            return await _context.Transferencias
                .Where(t => t.CheckinId == checkinId )
                .AsNoTracking()
                .CountAsync();
        }
        public async Task<int> GetCountByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.Transferencias
                .Where(t => t.DataTransferencia >= dataInicio && t.DataTransferencia <= dataFim)
                .AsNoTracking()
                .CountAsync();
        }
        public async Task<IEnumerable<Transferencia>> GetByHospedagemOrigemIdAsync(int hospedagemId)
        {
            return await _context.Transferencias
                .Include(t => t.Checkins)
                .Include(t => t.MotivoTransferencia)
                .Where(t => t.CheckinId == hospedagemId)
                .OrderByDescending(t => t.DataTransferencia)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transferencia>> GetByHospedagemDestinoIdAsync(int hospedagemId)
        {
            return await _context.Transferencias
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

        public async Task<IEnumerable<Transferencia>> GetByMotivoIdAsync(int motivoId)
        {
            return await _context.Transferencias
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

        public async Task<IEnumerable<Transferencia>> GetByPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.Transferencias
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

        public  async Task<Transferencia> GetByIdAsync(int id)
        {
            return await _context.Transferencias
                .Include(t => t.Checkins)
                .Include(t => t.MotivoTransferencia)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public  async Task<IEnumerable<Transferencia>> GetAllAsync()
        {
            return await _context.Transferencias
                .Include(t => t.Checkins)
                .Include(t => t.MotivoTransferencia)
                .OrderByDescending(t => t.DataTransferencia)
                .ToListAsync();
        }

    }
}