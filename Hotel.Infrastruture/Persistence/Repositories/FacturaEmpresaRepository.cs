using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Dtos;
using Hotel.Domain.DTOs;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Domain.Interface.Shared;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class FacturaEmpresaRepository : RepositoryBase<FacturaEmpresa>, IFacturaEmpresaRepository
    {
        private readonly GhotelDbContext _context;
        public FacturaEmpresaRepository(GhotelDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<IEnumerable<FacturaEmpresa>> GetFacturaEmpresaAsync()
        {
            return await _context.FacturaEmpresas
                              .Include(p => p.Empresas)
                              .Include(c => c.checkins).ThenInclude(h => h.Hospedagem).ThenInclude(a => a.Apartamentos)
                              .Include(c => c.checkins).ThenInclude(h => h.Hospedes).ThenInclude(c => c.Clientes)
                              .Include(c => c.checkins).ThenInclude(a => a.apartamentos)
                              .Include(c => c.checkins).ThenInclude(h => h.Pagamentos)
                              .ToListAsync();
        }
        public async Task<IEnumerable<object>> EmpresasComDividas()
        {
            return await _context.FacturaEmpresas
              .Include(e => e.Empresas)
           //   .Where(f => f.SituacaoFacturas == Domain.Enums.SituacaoFactura.Pendente)
              .GroupBy(f => new { f.EmpresasId, f.Empresas.RazaoSocial })
              .Select(g => new
              {
                  NomeEmpresa = g.Key.RazaoSocial,
                  EmpresasId = g.Key.EmpresasId,
                  QuantidadeDeFaturas = g.Count(),
                  ValorTotalDivida = g.Sum(f => f.Valor)
              })
              .ToListAsync();


        }

        public async Task<FacturaEmpresa> GetByIdAsync(int Id)
        {
            return await _context.FacturaEmpresas
            .Include(p => p.Empresas)
                               .Include(c => c.checkins).ThenInclude(h => h.Hospedagem).ThenInclude(a => a.Apartamentos)
                              .Include(c => c.checkins).ThenInclude(h => h.Hospedes).ThenInclude(c => c.Clientes)
                              .Include(c => c.checkins).ThenInclude(a => a.apartamentos)
                              .Include(c => c.checkins).ThenInclude(h => h.Pagamentos)
                              .FirstOrDefaultAsync(p => p.Id == Id);

        }

        /* public async Task<Empresa> GetEmpresasDividaAsync(int Id)
        {
            return await _context.Empresas
                               .Include(c=>c.F).ThenInclude(h=>h.Hospedagem)
                              .Include(c=>c.checkins).ThenInclude(h=>h.Hospedes)
                              .Include(c=>c.checkins).ThenInclude(a=>a.apartamentos)
                              .Include(c=>c.checkins).ThenInclude(h=>h.Pagamentos)
                              .FirstOrDefaultAsync(p => p.Id == Id);

        } */

        public async Task<FacturaEmpresa> GetByCheckinsIdAsync(int Id)
        {
            return await _context.FacturaEmpresas
            .Include(p => p.Empresas)
            .Include(c => c.checkins).ThenInclude(h => h.Hospedagem).ThenInclude(a => a.Apartamentos)
                              .Include(c => c.checkins).ThenInclude(h => h.Hospedes).ThenInclude(c => c.Clientes)
                              .Include(c => c.checkins).ThenInclude(a => a.apartamentos)
                              .Include(c => c.checkins).ThenInclude(h => h.Pagamentos)
            .FirstOrDefaultAsync(m => m.CheckinsId == Id);

        }

        public async Task<FacturaEmpresa> GetByIdEmpresaAsync(int Id)
        {
            return await _context.FacturaEmpresas
            .Include(p => p.Empresas)
            .Include(c => c.checkins).ThenInclude(h => h.Hospedagem).ThenInclude(a => a.Apartamentos)
                              .Include(c => c.checkins).ThenInclude(h => h.Hospedes).ThenInclude(c => c.Clientes)
                              .Include(c => c.checkins).ThenInclude(a => a.apartamentos)
                              .Include(c => c.checkins).ThenInclude(h => h.Pagamentos)
            .FirstOrDefaultAsync(m => m.EmpresasId == Id);

        }

        public async Task<IPaginatedList<FacturaEmpresa>> GetFilteredQuery(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {

            var query = _context.FacturaEmpresas
                                .Include(p => p.Empresas)
                                .Where(r => string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ||
                                       r.Id.ToString().Contains(paginationFilter.FieldFilter.ToLower()));

            return await IPaginatedList<FacturaEmpresa>.ToPagedList(query, paginationFilter.PageNumber, paginationFilter.PageSize);

        }

        public IQueryable<FacturaEmpresa> GetFilteredEmpresasDividasAsync(Domain.Interface.Shared.PaginationFilter paginationFilter, int Id)
        {
            /*  return _context.FacturaEmpresas
                                  .Include(p => p.Empresas).ThenInclude(m=>m.Clientes)
                                  .Include(c => c.checkins).ThenInclude(h => h.Hospedagem).ThenInclude(a=> a.Apartamentos)
                                     .Include(c => c.checkins).ThenInclude(h => h.Hospedes).ThenInclude(c=>c.Clientes)
                                     .Include(c => c.checkins).ThenInclude(a => a.apartamentos)
                                     .Include(c => c.checkins).ThenInclude(h => h.Pagamentos)

                                  .Where(r =>  string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) || r.EmpresasId.ToString().Contains(Id.ToString()) ||
                                         r.EmpresasId.ToString().Contains(paginationFilter.FieldFilter.ToLower())
                                         );
  */
            return _context.FacturaEmpresas
               .Include(p => p.Empresas).ThenInclude(e => e.Clientes)
               .Include(f => f.checkins).ThenInclude(c => c.Hospedagem).ThenInclude(h => h.Apartamentos)
               .Include(f => f.checkins).ThenInclude(c => c.Hospedes).ThenInclude(h => h.Clientes)
               .Include(f => f.checkins).ThenInclude(c => c.apartamentos)
               .Include(f => f.checkins).ThenInclude(c => c.Pagamentos)
               .Where(f =>
                   (string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ||
                   f.EmpresasId == Id) && // Verifica se a empresa corresponde ao ID especificado
                   (string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ||
                   f.Empresas.Clientes.Any(c => c.Nome.ToLower().Contains(paginationFilter.FieldFilter.ToLower()))) // Filtra pelos nomes dos clientes
               );

        }
        public IQueryable<FacturaEmpresa> GetFilteredDetalhesDeDividasEmpresaAsync(Domain.Interface.Shared.PaginationFilter paginationFilter, int Id)
        {

            /* return _context.FacturaEmpresas
               .Include(p => p.Empresas).ThenInclude(e => e.Clientes)
               .Include(f => f.checkins).ThenInclude(c => c.Hospedagem).ThenInclude(h => h.Apartamentos)
               .Include(f => f.checkins).ThenInclude(c => c.Hospedes).ThenInclude(h => h.Clientes)
               .Include(f => f.checkins).ThenInclude(c => c.apartamentos)
               .Include(f => f.checkins).ThenInclude(c => c.Pagamentos)
               .Where(f => f.EmpresasId == Id ||
                   
                   string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ||
                   f.Empresas.Clientes.Any(c => c.Nome.ToLower().Contains(paginationFilter.FieldFilter.ToLower())) // Filtra pelos nomes dos clientes
               ); */
               
               
               /* Ultimo alteração */
                  return _context.FacturaEmpresas
                        .Include(p => p.Empresas).ThenInclude(e => e.Clientes)
                        .Include(f => f.checkins).ThenInclude(c => c.Hospedagem).ThenInclude(h => h.Apartamentos)
                        .Include(f => f.checkins).ThenInclude(c => c.Hospedes).ThenInclude(h => h.Clientes)
                        .Include(f => f.checkins).ThenInclude(c => c.apartamentos)
                        .Include(f => f.checkins).ThenInclude(c => c.Pagamentos)
                        .Where(f=> f.EmpresasId == Id );// Filtra pela empresa específica)          

        }

        public IQueryable<FacturaEmpresaDetalhesDto> GetFilteredDetalhesDeDividasEmpresaAsyncV2(Domain.Interface.Shared.PaginationFilter paginationFilter, int Id)
{
    // ✅ CONSULTA OTIMIZADA COM DTO
    return _context.FacturaEmpresas
        .Where(f => f.EmpresasId == Id) // ✅ FILTRO PRIMEIRO
        .Where(f => string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) || 
     //              f.Empresas.Clientes.Any(c => c.Nome.ToLower().Contains(paginationFilter.FieldFilter.ToLower())))
     f.checkins.Hospedes.Any(c => c.Clientes.Nome.ToLower().Contains(paginationFilter.FieldFilter.ToLower())))

        .Select(f => new FacturaEmpresaDetalhesDto
        {
            Id = f.Id,
            EmpresasId = f.EmpresasId,
            CheckinsId = f.CheckinsId,
            Total = f.Valor,
            
            // ✅ DADOS ESSENCIAIS EM UMA CONSULTA
            RazaoSocialEmpresa = f.Empresas.RazaoSocial,
            NomeCliente = f.Empresas.Clientes.FirstOrDefault().Nome ?? "N/A",
            EmailCliente = f.Empresas.Clientes.FirstOrDefault().Email ?? "N/A",
            
            DataEntrada = f.checkins.DataEntrada,
            DataSaida = f.checkins.DataSaida,   
            Quarto = f.checkins.Hospedagem.FirstOrDefault().Apartamentos.Codigo ?? "N/A",
            QuantidadeDeDiarias = f.checkins.Hospedagem.FirstOrDefault().QuantidadeDeDiarias,
            situacaoFacturas = f.SituacaoFacturas.ToString(),
            ValorDiaria = f.checkins.Hospedagem.FirstOrDefault().ValorDiaria,
            NumeroFactura = f.Tipo + ' ' + f.NumeroFactura.ToString() + '/' + f.Ano.ToString(),
            Hospede = f.checkins.Hospedes.FirstOrDefault().Clientes.Nome ?? "N/A",
            QuantidadeHospedes = f.checkins.Hospedes.Count(),
          //  NumeroFactura = f.Tipo + ' ' +  f.NumeroFactura.ToString() + '/' + f.Ano.ToString(),
          //  Hospede = f.checkins.Hospedes.FirstOrDefault().Clientes.Nome ?? "N/A",      
          //  QuantidadeHospedes = f.checkins.Hospedes.Count()
        });
}
        public (IQueryable<FacturaEmpresa> Registros, float ValorTotal) GetFilteredDetalhesDeDividasEmpresaComTotalAsync(Domain.Interface.Shared.PaginationFilter paginationFilter, int Id)
        {
            // Filtro principal
            var query = _context.FacturaEmpresas
                .Include(p => p.Empresas).ThenInclude(e => e.Clientes)
                .Include(f => f.checkins).ThenInclude(c => c.Hospedagem).ThenInclude(h => h.Apartamentos)
                .Include(f => f.checkins).ThenInclude(c => c.Hospedes).ThenInclude(h => h.Clientes)
                .Include(f => f.checkins).ThenInclude(c => c.apartamentos)
                .Include(f => f.checkins).ThenInclude(c => c.Pagamentos)
                .Where(f =>
                    f.EmpresasId == Id && // Filtra pela empresa específica
                    (string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ||
                    f.Empresas.Clientes.Any(c => c.Nome.ToLower().Contains(paginationFilter.FieldFilter.ToLower()))) // Filtra pelos nomes dos clientes
                );

            // Calcula o valor total das faturas filtradas
            float valorTotal = query.Sum(f => f.Valor);

            // Retorna a consulta e o valor total
            return (query, valorTotal);
        }

        public IQueryable<FacturaEmpresa> GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            return _context.FacturaEmpresas
                                 .Include(p => p.Empresas)
                                 .Include(c => c.checkins).ThenInclude(h => h.Hospedagem).ThenInclude(a => a.Apartamentos)
                                    .Include(c => c.checkins).ThenInclude(h => h.Hospedes).ThenInclude(c => c.Clientes)
                                    .Include(c => c.checkins).ThenInclude(a => a.apartamentos)
                                    .Include(c => c.checkins).ThenInclude(h => h.Pagamentos)
                                 .Where(r => string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ||
                                        r.EmpresasId.ToString().Contains(paginationFilter.FieldFilter.ToLower()));

        }
        public IQueryable<FacturaEmpresaDto> GetFilteredEmpresasComDividasAsync(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {

            return _context.FacturaEmpresas
               .Include(e => e.Empresas)
               .Where(f =>
                  // f.SituacaoFacturas == Domain.Enums.SituacaoFactura.Pendente && // Filtro por situação
                   (string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ||
                   f.Empresas.RazaoSocial.ToLower().Contains(paginationFilter.FieldFilter.ToLower())) // Filtro por razão social
               )
               .GroupBy(f => new { f.EmpresasId, f.Empresas.RazaoSocial })
               .Select(g => new FacturaEmpresaDto
               {
                   EmpresasId = g.Key.EmpresasId,
                   NomeEmpresa = g.Key.RazaoSocial,
                   QuantidadeDeFaturas = g.Count(),
                   ValorTotalDivida = g.Sum(f => f.Valor)
               });


        }
        public async Task<int> AddEmpresaAsync(FacturaEmpresa checkins)
        {
            _context.FacturaEmpresas.Add(checkins);
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
    }
}