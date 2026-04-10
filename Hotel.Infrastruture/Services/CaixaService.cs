
using Hotel.Application.Interfaces;
using Hotel.Infrastruture.Persistence.Context;

namespace Hotel.Infrastruture.Services
{
    public class CaixaService : ICaixa
    {
        private readonly GhotelDbContext _context;
       
        private DateTime today;
        private DateTime tomorrow;
        public CaixaService(GhotelDbContext context)
        {
            _context = context;
              today = DateTime.Now.Date;
             tomorrow = today.AddDays(1);
        }


    

/* var caixa = await _context.Caixas
    .FirstOrDefaultAsync(c => c.DataDeAbertura >= today && c.DataDeAbertura < tomorrow);

 */        public int getCaixa => int.Parse(_context.Caixas
                                .Where(c => c.DataDeAbertura.Date >= today && c.DataDeAbertura < tomorrow)
                                .Select(m => m.Id.ToString())
                                .FirstOrDefault() ?? "0");

        /*     _context.AppConfig
                    .Where(entry => entry.Key == key)
                    .Select(entry => entry.Value)
                    .FirstOrDefault(); */

    }
}