
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Domain.Interface.Shared;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
  public class CheckinRepository : RepositoryBase<Checkins>, ICheckinRepository
  {
     private readonly GhotelDbContext _context;
        public CheckinRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Checkins>> GetApartamentoAsync()
    {
       var checkins =  await _context.Checkins

                        .Include(p => p.Hospedagem)
                       // .Include(p=>p.Pagamentos)
                        .ToListAsync();
 // Se precisar carregar pagamentos para todos os checkins
    if (checkins.Any())
    {
        var checkinIds = checkins.Select(c => c.Id).ToList();
        
        var pagamentos = await _context.Pagamentos
            .Where(p => p.Origem == "Diarias" && checkinIds.Contains(p.OrigemId))
            .Include(p => p.Hospedes)
            .Include(p => p.Utilizadores)
            .ToListAsync();

        // Associar pagamentos aos respectivos checkins
        foreach (var checkin in checkins)
        {
            // Se a propriedade Pagamentos existe em Checkins
            // checkin.Pagamentos = pagamentos.Where(p => p.OrigemId == checkin.Id).ToList();
        }
    }

        return checkins;
    }
    public async Task<Checkins> GetByIdAsync(int Id)
    {
      var checkin = await _context.Checkins

                        .Include(p => p.Hospedagem)
                       // .Include(p=>p.Pagamentos)
                        .Include(p=>p.Hospedes).ThenInclude(p=>p.Clientes).ThenInclude(p=>p.Empresa)
                        .FirstOrDefaultAsync(p => p.Id == Id);
 if (checkin != null)
    {
        // Carregar pagamentos usando Origem e OrigemId
        var pagamentos = await _context.Pagamentos
            .Where(p => p.Origem == "Diarias" && p.OrigemId == Id)
            .Include(p => p.Hospedes)
            .Include(p => p.Utilizadores)
            .ToListAsync();

        // Associar os pagamentos ao checkin (se a propriedade ainda existir)
        // Se Checkins.Pagamentos ainda existe na entidade, faça:
        // checkin.Pagamentos = pagamentos;
    }
     
     return checkin;

    }

    public async Task<IPaginatedList<Checkins>> GetFilteredApartamentoquery(Domain.Interface.Shared.PaginationFilter paginationFilter)
    {
      /* var aux = await IPaginatedList<Checkins>.ToPagedList(
       _context.Checkins

                           .Include(p => p.Hospedagem)
                           .Where(r => r.Id.ToString().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ? paginationFilter.FieldFilter.ToLower() : "")
                           //   .Where(r=>r.Id == (paginationFilter.FieldFilter ?? r.Id )  ) 
                           )
     //      .ToListAsync();
     , paginationFilter.PageNumber, paginationFilter.PageSize);

      return aux; */
      var query = _context.Checkins
                                .Include(p => p.Hospedagem)
                                .Where(r => string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ||
                                            r.Id.ToString().Contains(paginationFilter.FieldFilter.ToLower()));

      return await IPaginatedList<Checkins>.ToPagedList(query, paginationFilter.PageNumber, paginationFilter.PageSize);

    }

    public IQueryable<Checkins> GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter)
    {
      /*  IQueryable<Checkins> query = Enumerable.Empty<Checkins>().AsQueryable();
       query = (from apart in _context.Checkins

                                .Include(p => p.Hospedagem)
                                .Where(r => r.Id.ToString().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ? paginationFilter.FieldFilter.ToLower() : ""
                                ))
                select apart);
       return query; */

      return _context.Checkins
                           .Include(p => p.Hospedagem)
                           .Include(p=>p.Pagamentos)
                           .Where(r => string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ||
                                       r.Id.ToString().Contains(paginationFilter.FieldFilter.ToLower()));

    }

    // Hotel.Infrastruture/Persistence/Shared/RepositoryBase.cs - implementar
    public async Task<int> ExecuteSqlRawAsync(string sql, params object[] parameters)
    {
        return await _context.Database.ExecuteSqlRawAsync(sql, parameters);
    }
    public async Task<int> AddCheckinsAsync(Checkins checkins)
    {


      //  await AddHospedagemAsync(checkins.Hospedagens);
      _context.Checkins.Add(checkins);
      await _context.SaveChangesAsync();

      return checkins.Id;
    }

    public async Task AddHospedagemAsync(Hospedagem hospedagem)
    {

      var ch = hospedagem.Checkins;

      _context.Hospedagems.Add(hospedagem);
      await _context.SaveChangesAsync();
      //   return hospedagem;
    }

    public void AtualizarPrevisaoParaAtrasados()
    {
      throw new NotImplementedException();
    }

    public async Task<List<Checkins>> GetCheckinsByDateAsync(DateTime date)
    {
      return await _context.Checkins
          .Include(c => c.Hospedagem)
              .ThenInclude(h => h.Apartamentos)
          .Include(c => c.Hospedagem)
              .ThenInclude(h => h.Empresas)
          .Include(c => c.Hospedes)
              .ThenInclude(h => h.Clientes)
          .Include(c => c.UtilizadoresCheckin)
          .Where(c => c.DataEntrada.Date == date.Date)
          .OrderBy(c => c.Id)
          .AsNoTracking()
          .ToListAsync();
    }

    public async Task<List<Checkins>> GetCheckoutsByDateAsync(DateTime date)
    {
      return await _context.Checkins
          .Include(c => c.Hospedagem)
              .ThenInclude(h => h.Apartamentos)
          .Include(c => c.Hospedagem)
              .ThenInclude(h => h.Empresas)
          .Include(c => c.Hospedes)
              .ThenInclude(h => h.Clientes)
          .Include(c => c.UtilizadoresCheckout)
          .Where(c => c.CheckoutRealizado && c.DataSaida.HasValue && c.DataSaida.Value.Date == date.Date)
          .OrderBy(c => c.Id)
          .AsNoTracking()
          .ToListAsync();
    }

        public async Task<List<Checkins>> GetCheckinsFechamentoCaixaAsync(DateTime date, int? caixaId)
        {
          var query = _context.Checkins
            .Include(c => c.Hospedagem)
              .ThenInclude(h => h.Apartamentos)
            .Include(c => c.Hospedagem)
              .ThenInclude(h => h.Empresas)
            .Include(c => c.Hospedes)
              .ThenInclude(h => h.Clientes)
            .Include(c => c.UtilizadoresCheckin)
            .Where(c => c.DataEntrada.Date == date.Date && !c.CheckoutRealizado)
            .AsQueryable();

          if (caixaId.HasValue)
            query = query.Where(c => c.IdCaixaCheckin == caixaId.Value);

          return await query
            .OrderBy(c => c.Id)
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<List<Checkins>> GetCheckoutsFechamentoCaixaAsync(DateTime date, int? caixaId)
        {
          var query = _context.Checkins
            .Include(c => c.Hospedagem)
              .ThenInclude(h => h.Apartamentos)
            .Include(c => c.Hospedagem)
              .ThenInclude(h => h.Empresas)
            .Include(c => c.Hospedes)
              .ThenInclude(h => h.Clientes)
            .Include(c => c.UtilizadoresCheckout)
            .Where(c => c.CheckoutRealizado && c.Hospedagem.Any(h => h.PrevisaoFechamento.Date == date.Date))
            .AsQueryable();

          if (caixaId.HasValue)
            query = query.Where(c => c.IdCaixaCheckOut == caixaId.Value);

          return await query
            .OrderBy(c => c.Id)
            .AsNoTracking()
            .ToListAsync();
        }

    /*   public void RealizarCheckout(int checkinId, DateTime dataSaida)
      {
        var checkin = _context.Checkins.FirstOrDefault(m => m.Id == checkinId);

        if (checkin == null)
          throw new ArgumentException("Check-in não encontrado.");

        checkin.RegistrarSaida(dataSaida);
        checkin.RealizarCheckout();

        _context.Checkins.Update(checkin);
      } */

    public async Task RealizarCheckoutAsync(int checkinId, int hospedeId, DateTime dataHoraSaida)
    {
      // Obtendo os dados necessários
      var checkin = await _context.Checkins
                                  .Include(m => m.Pagamentos)
                                  .Include(h => h.Hospedagem)
                                  .AsNoTracking()
                                  .FirstOrDefaultAsync(m => m.Id == checkinId);

/*       if (checkin == null)
        throw new ArgumentException("Check-in não encontrado.");

      var hospedagem = await _context.Hospedagems
                                     .FirstOrDefaultAsync(m => m.CheckinsId == checkinId);

      if (hospedagem == null)
        throw new ArgumentException("Hospedagem não encontrada.");

      var hospede = await _context.Hospedes
                                  .FirstOrDefaultAsync(h => h.Id == hospedeId);



      if (hospede == null)
        throw new ArgumentException("Hóspede não encontrado.");

      /*  if (!checkin.Pagamentos.Any())
         throw new ArgumentException("Não é possível realizar o checkout sem pagamentos.");
  */
    /*   if (dataHoraSaida.Date < hospedagem.PrevisaoFechamento.Date)
        throw new ApplicationException("A data de saída não pode ser menor que a data de previsão de fechamento.");

      if (!hospede.PodeFazerCheckout(checkin.situacaoDoPagamento))
        throw new ArgumentException("O hóspede não pode fazer o checkout sem pagar."); 

      // Atualizando informações
      hospedagem.FecharHospedagem(dataHoraSaida);
      _context.Hospedagems.Update(hospedagem);
      _context.ChangeTracker.Clear();
      await _context.SaveChangesAsync(); */


/*       var facturaEmpresa = new FacturaEmpresa(checkinId, 5, checkin.ValorTotalDiaria, hospedagem.EmpresasId, dataHoraSaida);

      if (hospede.Estado == Hospede.EstadoHospede.Empresa)
      {
        await _context.FacturaEmpresas.AddAsync(facturaEmpresa);
      } */
      // Persistindo alterações
      //  _context.Hospedagems.Update(hospedagem);
      _context.Checkins.Update(checkin);
      await _context.SaveChangesAsync();
    }
  }
}