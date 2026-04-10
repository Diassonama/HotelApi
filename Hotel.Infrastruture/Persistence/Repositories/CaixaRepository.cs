using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using Hotel.Domain.Interface.Shared;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Serilog;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class CaixaRepository : RepositoryBase<Caixa>, ICaixaRepository
    {
        private readonly GhotelDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UsuarioLogado _usuarioLogado;

        public CaixaRepository(GhotelDbContext context, IHttpContextAccessor httpContextAccessor, UsuarioLogado usuarioLogado) : base(context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _usuarioLogado = usuarioLogado;
        }

        /// <summary>
        /// Obtém o ID do usuário atual logado
        /// </summary>
        /// 
        /// 
        /// 
        private string ObterUsuarioAtualId()
        {


         //   var identity = _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
         //   var userId2 = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // The following code is not valid in this repository context and references undefined variables/methods.
            // If you need to handle unauthorized access, do it at the service/controller layer.
            // Remove the invalid code below:

            // if (string.IsNullOrEmpty(usuarioId))
            // {
            //     resposta.Success = false;
            //     resposta.Message = "❌ Usuário não autenticado";
            //     return Unauthorized(resposta);
            // }

            // var utilizador = await _authService.Usuariologado(_usuarioLogado.IdUtilizador);


          //  var userId2 = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
          //  return userId ?? string.Empty;
            


             try
            {
                /*                 var httpContext = _httpContextAccessor?.HttpContext;
                                if (httpContext == null)
                                {
                                    Log.Warning("HttpContext é nulo - método chamado fora de contexto HTTP");
                                    return string.Empty;
                                }

                                var user = httpContext.User;
                                if (user == null)
                                {
                                    Log.Warning("User é nulo no HttpContext");
                                    return string.Empty;
                                }

                                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value; */
                Log.Warning("Não foi possível obter o ID do usuário atual" , _usuarioLogado.IdUtilizador);
                return _usuarioLogado.IdUtilizador ?? string.Empty;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao obter ID do usuário atual");
                return string.Empty;
            }
        }

        /// <summary>
        /// Verifica se o usuário atual é administrador
        /// </summary>
        private bool EhAdministrador(string perfil)
        {
            /*  var roles = _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)?.Select(c => c.Value);
             return roles?.Any(r => r.Equals("Administrador", StringComparison.OrdinalIgnoreCase) ||
                                   r.Equals("Admin", StringComparison.OrdinalIgnoreCase)
                                   || r.Equals("Finanças", StringComparison.OrdinalIgnoreCase)
                                   || r.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase)) ?? false;
   */      
  try
            {


                //verifica se o perfil do usuario é admin,administrador,finanças ou superadmin retornando true
                if (!string.IsNullOrEmpty(perfil))
                {
                    var perfilLower = perfil.ToLower();
                    if (perfilLower == "administrador" || perfilLower == "admin" || perfilLower == "finanças" || perfilLower == "superadmin")
                    {
                        return true;
                    }
                }    

               

                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao verificar se usuário é administrador - assumindo não-admin");
                return false;
            }
   }

 private bool TemContextoHttpValido()
        {
            try
            {
                return _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<Caixa> GetByIdAsync(int id)
        {
            IQueryable<Caixa> query = _context.Caixas.Include(c => c.Checkins);

           /*  if (!EhAdministrador(perfil))
            {
               
                query = query.Include(c => c.LancamentoCaixas.Where(l => l.UtilizadoresId == usuarioId));
            }
            else
            { */
                query = query.Include(c => c.LancamentoCaixas);
       //     }

            return await query.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Caixa> GetByDateAsync(DateTime data, string usuarioId, string perfil)
        {
            IQueryable<Caixa> query = _context.Caixas.Include(c => c.Checkins);

    if (!EhAdministrador(perfil))
    {
       
        query = query.Include(c => c.LancamentoCaixas.Where(l => l.UtilizadoresId == usuarioId));
    }
    else
    {
        query = query.Include(c => c.LancamentoCaixas);
    }

    return await query.FirstOrDefaultAsync(c => c.DataDeAbertura.Date == data.Date);
        }

        public async Task<int> getCaixa()
        {
            var today = DateTime.Now.Date;
            var tomorrow = today.AddDays(1);

            // ✅ ADMINISTRADORES PODEM VER QUALQUER CAIXA, OUTROS APENAS OS PRÓPRIOS
            var query = _context.Caixas.AsQueryable();

          /*   if (!EhAdministrador())
            {
                var usuarioId = ObterUsuarioAtualId();
                query = query.Where(c => c.LancamentoCaixas.Any(l => l.UtilizadoresId == usuarioId));
            } */

            var caixa = await query
                .FirstOrDefaultAsync(c => c.DataDeAbertura >= today && c.DataDeAbertura < tomorrow);

            return caixa?.Id ?? 0;
        }

        public new async Task<IEnumerable<Caixa>> GetAllAsync(string usuarioId, string perfil)
        {
            IQueryable<Caixa> query;

            // ✅ FILTRAR POR USUÁRIO SE NÃO FOR ADMINISTRADOR
            if (!EhAdministrador(perfil))
            {
               
                query = _context.Caixas
                                .Where(c => c.LancamentoCaixas.Any(l => l.UtilizadoresId == usuarioId))
                                .Include(c => c.LancamentoCaixas)
                                .Include(c => c.Checkins);
            }
            else
            {
                query = _context.Caixas
                                .Include(c => c.LancamentoCaixas)
                                .Include(c => c.Checkins);
            }

            return await query.ToListAsync();
        }

        public async Task<IPaginatedList<Caixa>> GetFilteredquery(Domain.Interface.Shared.PaginationFilter paginationFilter, string usuarioId, string perfil)
        {
            var query = _context.Caixas.Include(c => c.LancamentoCaixas)
                                      .Include(c => c.Checkins);

            // ✅ APLICAR FILTRO DE USUÁRIO SE NÃO FOR ADMINISTRADOR
            if (!EhAdministrador(perfil))
            {
               
                query = query.Where(c => c.LancamentoCaixas.Any(l => l.UtilizadoresId == usuarioId))
                             .Include(c => c.LancamentoCaixas)
                             .Include(c => c.Checkins);
            }

            // ✅ APLICAR FILTRO DE PESQUISA
            if (!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter))
            {
                var filteredQuery = query.Where(r => r.Id.ToString().Contains(paginationFilter.FieldFilter.ToLower()));
                query = filteredQuery.Include(c => c.LancamentoCaixas)
                                    .Include(c => c.Checkins);
            }

            var aux = await IPaginatedList<Caixa>.ToPagedList(
                query, paginationFilter.PageNumber, paginationFilter.PageSize);

            return aux;
        }


     public   IQueryable<Caixa> GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter, string usuarioId, string perfil)
{
    IQueryable<Caixa> query = _context.Caixas;

    // ✅ APLICAR FILTRO DE USUÁRIO SE NÃO FOR ADMINISTRADOR
    if (!EhAdministrador(perfil))
    {
       

        // Primeiro aplica o filtro de usuário, depois os includes
        query = query.Where(c => c.LancamentoCaixas.Any(l => l.UtilizadoresId == usuarioId))
                     .Include(c => c.LancamentoCaixas.Where(l => l.UtilizadoresId == usuarioId))
                     .Include(c => c.Checkins);
    }
    else
    {
        // Para administradores, inclui todos os lançamentos
        query = query.Include(c => c.LancamentoCaixas)
                     .Include(c => c.Checkins);
    }

    // ✅ APLICAR FILTRO DE PESQUISA
    if (!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter))
    {
        query = query.Where(r => r.Id.ToString().Contains(paginationFilter.FieldFilter.ToLower()));
    }

    return query;
}

        public async Task<object> MovimentoDoCaixa(string usuarioId, string perfil)
        {
            // ✅ OBTER MOVIMENTOS DO CAIXA COM FILTRO POR USUÁRIO
            var query = _context.LancamentoCaixas
                .Include(m => m.Utilizadores)
                .Include(m => m.TipoPagamentos)
                .Where(m => m.DataHoraLancamento.Date == DateTime.UtcNow.Date);

            // ✅ FILTRAR POR USUÁRIO SE NÃO FOR ADMINISTRADOR
            if (!EhAdministrador(perfil))
            {
               
                query = query.Where(m => m.UtilizadoresId == usuarioId);
            }

            query = query.OrderByDescending(m => m.PagamentosId);

            // ✅ CÁLCULOS DO DIA (com filtro por usuário se necessário)
            var queryDia = _context.LancamentoCaixas
                .Where(m => m.DataHoraLancamento.Date == DateTime.UtcNow.Date);

            if (!EhAdministrador(perfil))
            {
                
                queryDia = queryDia.Where(m => m.UtilizadoresId == usuarioId);
            }

            double totalEntradas = queryDia
                .Where(m => m.TipoLancamento == Domain.Enums.TipoLancamento.E)
                .Sum(m => (double)m.ValorPago);

            double totalSaidas = queryDia
                .Where(m => m.TipoLancamento == Domain.Enums.TipoLancamento.S)
                .Sum(m => (double)m.ValorPago);

            double saldoDia = totalEntradas - totalSaidas;

            // ✅ CÁLCULOS DO MÊS (com filtro por usuário se necessário)
            var queryMes = _context.LancamentoCaixas
                .Where(m => m.DataHoraLancamento.Month == DateTime.UtcNow.Month && 
                           m.DataHoraLancamento.Year == DateTime.UtcNow.Year);

            if (!EhAdministrador(perfil))
            {
                
                queryMes = queryMes.Where(m => m.UtilizadoresId == usuarioId);
            }

            double totalEntradasMes = queryMes
                .Where(m => m.TipoLancamento == Domain.Enums.TipoLancamento.E)
                .Sum(m => (double)m.ValorPago);

            double totalSaidasMes = queryMes
                .Where(m => m.TipoLancamento == Domain.Enums.TipoLancamento.S)
                .Sum(m => (double)m.ValorPago);

            double saldoMes = totalEntradasMes - totalSaidasMes;

            // ✅ INFORMAÇÕES ADICIONAIS PARA IDENTIFICAR O CONTEXTO
            var usuarioAtual = ObterUsuarioAtualId();
            var ehAdmin = EhAdministrador(perfil);

            return new
            {
                totalEntradas,
                totalSaidas,
                saldoDia,
                saldoMes,
                totalEntradasMes,
                totalSaidasMes,
                // ✅ METADADOS PARA CONTROLE
                UsuarioId = usuarioAtual,
                EhAdministrador = ehAdmin,
                FiltradoPorUsuario = !ehAdmin,
                DataConsulta = DateTime.UtcNow,
                Movimento = await query.ToListAsync()
            };
        }

        /// <summary>
        /// ✅ NOVO MÉTODO: Obter movimento do caixa de um usuário específico (apenas para administradores)
        /// </summary>
        public async Task<object> MovimentoDoCaixaPorUsuario(string usuarioId, string perfil)
        {
            try
    {
        // ✅ VALIDAÇÃO DE ENTRADA
       /*  if (usuarioId )
        {
            throw new ArgumentException("ID do usuário deve ser maior que zero", nameof(usuarioId));
        } */

        // ✅ APENAS ADMINISTRADORES PODEM VER DADOS DE OUTROS USUÁRIOS
        if (!EhAdministrador(perfil))
        {
            throw new UnauthorizedAccessException("Apenas administradores podem consultar dados de outros usuários");
        }

        // ✅ VERIFICAR SE O USUÁRIO EXISTE
        var usuarioExiste = await _context.Utilizadores.AnyAsync(u => u.Id == usuarioId);
        if (!usuarioExiste)
        {
            throw new ArgumentException($"Usuário com ID {usuarioId} não encontrado", nameof(usuarioId));
        }

        // ✅ CONVERTER usuarioId para string para comparação (se necessário)
        var usuarioIdString = usuarioId.ToString();

        var hoje = DateTime.UtcNow.Date;
        var mesAtual = DateTime.UtcNow.Month;
        var anoAtual = DateTime.UtcNow.Year;

        // ✅ QUERY PARA MOVIMENTOS DO DIA
        var queryMovimentosDia = _context.LancamentoCaixas
            .Include(m => m.Utilizadores)
            .Include(m => m.TipoPagamentos)
            .Where(m => m.DataHoraLancamento.Date == hoje && 
                       (m.UtilizadoresId == usuarioIdString || m.UtilizadoresId == usuarioId.ToString()))
            .OrderByDescending(m => m.PagamentosId);

        // ✅ QUERY BASE PARA CÁLCULOS (todos os lançamentos do usuário)
        var queryUsuario = _context.LancamentoCaixas
            .Where(m => m.UtilizadoresId == usuarioIdString || m.UtilizadoresId == usuarioId.ToString());

        // ✅ CÁLCULOS DO DIA
        var entradasDiaQuery = queryUsuario
            .Where(m => m.DataHoraLancamento.Date == hoje && 
                       m.TipoLancamento == Domain.Enums.TipoLancamento.E);

        var saidasDiaQuery = queryUsuario
            .Where(m => m.DataHoraLancamento.Date == hoje && 
                       m.TipoLancamento == Domain.Enums.TipoLancamento.S);

        double totalEntradasDia = await entradasDiaQuery.AnyAsync() ? 
            await entradasDiaQuery.SumAsync(m => (double)m.ValorPago) : 0;

        double totalSaidasDia = await saidasDiaQuery.AnyAsync() ? 
            await saidasDiaQuery.SumAsync(m => (double)m.ValorPago) : 0;

        // ✅ CÁLCULOS DO MÊS
        var entradasMesQuery = queryUsuario
            .Where(m => m.DataHoraLancamento.Month == mesAtual && 
                       m.DataHoraLancamento.Year == anoAtual &&
                       m.TipoLancamento == Domain.Enums.TipoLancamento.E);

        var saidasMesQuery = queryUsuario
            .Where(m => m.DataHoraLancamento.Month == mesAtual && 
                       m.DataHoraLancamento.Year == anoAtual &&
                       m.TipoLancamento == Domain.Enums.TipoLancamento.S);

        double totalEntradasMes = await entradasMesQuery.AnyAsync() ? 
            await entradasMesQuery.SumAsync(m => (double)m.ValorPago) : 0;

        double totalSaidasMes = await saidasMesQuery.AnyAsync() ? 
            await saidasMesQuery.SumAsync(m => (double)m.ValorPago) : 0;

        // ✅ OBTER DADOS DO USUÁRIO
        var usuario = await _context.Utilizadores.FindAsync(usuarioId);

        // ✅ OBTER MOVIMENTOS DO DIA
        var movimentosDia = await queryMovimentosDia.ToListAsync();

        // ✅ ESTATÍSTICAS ADICIONAIS
        var totalMovimentosDia = movimentosDia.Count;
        var ultimoMovimento = movimentosDia.FirstOrDefault();

        return new
        {
            UsuarioId = usuarioId,
            NomeUsuario = usuario?.FirstName + " " + usuario?.LastName ?? "Usuário não encontrado",
            EmailUsuario = usuario?.Email ?? "N/A",
            
            // Dados do dia
            TotalEntradasDia = totalEntradasDia,
            TotalSaidasDia = totalSaidasDia,
            SaldoDia = totalEntradasDia - totalSaidasDia,
            TotalMovimentosDia = totalMovimentosDia,
            
            // Dados do mês
            TotalEntradasMes = totalEntradasMes,
            TotalSaidasMes = totalSaidasMes,
            SaldoMes = totalEntradasMes - totalSaidasMes,
            
            // Metadados
            DataConsulta = DateTime.UtcNow,
            DataReferencia = hoje,
            MesReferencia = $"{mesAtual:00}/{anoAtual}",
            ConsultadoPorAdmin = true,
            UltimoMovimento = ultimoMovimento?.DataHoraLancamento,
            
            // Movimentos detalhados
            Movimento = movimentosDia.Select(m => new
            {
                Id = m.Id,
                Valor = m.ValorPago,
                TipoLancamento = m.TipoLancamento.ToString(),
                DataHora = m.DataHoraLancamento,
                TipoPagamento = m.TipoPagamentos?.Descricao ?? "N/A",
                Descricao = m.Observacao ?? "N/A"
            }).ToList()
        };
    }
    catch (ArgumentException ex)
    {
        throw new ArgumentException($"Erro de validação: {ex.Message}", ex);
    }
    catch (UnauthorizedAccessException ex)
    {
        throw new UnauthorizedAccessException($"Acesso negado: {ex.Message}", ex);
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException($"Erro ao consultar movimento do caixa do usuário {usuarioId}: {ex.Message}", ex);
    }
}

       
    }
}