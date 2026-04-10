using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class HospedeRepository : RepositoryBase<Hospede>, IHospedeRepository
    {
        private readonly GhotelDbContext _context; 
        public HospedeRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }
            public async Task<Hospede> GetByIdAsync(int id)
        {
            return await _context.Hospedes
                              .Include(p => p.checkins)
                              .Include(p => p.Clientes)         
                              .FirstOrDefaultAsync(p => p.Id == id);
        }

            public async Task<Hospede> GetByCheckinIdAsync(int id)
        {
            return await _context.Hospedes
                              .Include(p => p.checkins)
                              .Include(p => p.Clientes)         
                              .FirstOrDefaultAsync(p => p.CheckinsId == id);
        }
    }
}