using System.Data.Entity.SqlServer.Utilities;
using System.Collections;
using Hotel.Domain.Dtos;
using Hotel.Domain.Entities;
using Hotel.Domain.Enums;
using Hotel.Domain.Interface;
using Hotel.Domain.Interface.Shared;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Infrastruture.Persistence.Shared;
using Microsoft.EntityFrameworkCore;

//using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Persistence.Repositories
{
    public class ApartamentoRepository : RepositoryBase<Apartamentos>, IApartamentoRepository
    {
        private readonly GhotelDbContext _context;

        public ApartamentoRepository(GhotelDbContext dbContext, GhotelDbContext context) : base(dbContext)
        {
            _context = context;
        }

        /*         public ApartamentoRepository(GhotelDbContext context, GhotelDbContext contexto) : base(context)
       {
           _contexto = contexto;
       } */

        public Task<IPaginatedList<Apartamentos>> ConsultaTodosWithPagging(int page, int pagesize, string searchTerm)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Apartamentos>> GetApartamentoAsync()
        {


            return await _context.Apartamentos
                                                      .Include(p => p.TipoApartamentos)
                                                      .Include(p => p.TipoGovernancas)
                                                      .Include(h => h.checkins).ThenInclude(h => h.Hospedes).ThenInclude(c => c.Clientes)

                                                      .Include(h => h.checkins).ThenInclude(h => h.Hospedagem).ThenInclude(c => c.Empresas)
                                                      .Include(h => h.checkins)
                                                      
                                                      //.ThenInclude(h => h.Pagamentos)
                                                      .AsNoTracking()


                                                      .ToListAsync();
        }

        public async Task<IEnumerable<Apartamentos>> GetApartamentoOcupadosAsync()
        {


            return await _context.Apartamentos
                              .Include(p => p.TipoApartamentos)
                              .Include(p => p.TipoGovernancas)
                              .Include(h => h.checkins).ThenInclude(h => h.Hospedes).ThenInclude(c => c.Clientes)

                              .Include(h => h.checkins).ThenInclude(h => h.Hospedagem).ThenInclude(c => c.Empresas)
                              //.Include(h => h.checkins)//.ThenInclude(h => h.Pagamentos)
                               .Include(h => h.checkins).ThenInclude(c => c.Pagamentos).ThenInclude(p => p.Hospedes)
                              .Include(h => h.checkins).ThenInclude(c => c.Pagamentos).ThenInclude(p => p.LancamentoCaixas)

                              .Where(h => h.Situacao != Situacao.Livre)
                              .AsNoTracking()
                              .ToListAsync();
        }
        public async Task<Apartamentos> GetByIdAsync(int Id)
        {
            return await _context.Apartamentos
                              .Include(p => p.TipoApartamentos)
                              .Include(p => p.TipoGovernancas)
                              .Include(h => h.checkins).ThenInclude(h => h.Hospedes).ThenInclude(c => c.Clientes)
                              .Include(h => h.checkins).ThenInclude(h => h.Hospedagem).ThenInclude(c => c.Empresas)
                              .Include(h => h.checkins).ThenInclude(c => c.Pagamentos).ThenInclude(p => p.Hospedes)
                              .Include(h => h.checkins).ThenInclude(c => c.Pagamentos).ThenInclude(p => p.LancamentoCaixas)
                              // .AsNoTracking()
                              .FirstOrDefaultAsync(p => p.Id == Id);

        }
        public async Task<List<Apartamentos>> GetBySituacaoAsync(Situacao situacao)
        {
            return await _context.Apartamentos
                                 .Include(p => p.TipoApartamentos)
                                 //   .Include(p => p.TipoGovernancas)
                                 .Include(h => h.checkins).ThenInclude(h => h.Hospedes).ThenInclude(c => c.Clientes)

                                 .Include(h => h.checkins).ThenInclude(h => h.Hospedagem).ThenInclude(c => c.Empresas)
                                 .Include(h => h.checkins)//.ThenInclude(h => h.Pagamentos)
                                 .Where(p => p.Situacao == situacao)
                                 .ToListAsync();
        }

        public async Task<List<Apartamentos>> GetQuartosAtrazadosAsync()
        {
            var dataAtual = DateTime.Now.Date;

            var apartamentos = await _context.Apartamentos
                // .Include(p => p.TipoApartamentos)
                // .Include(p => p.TipoGovernancas)
                //   .Include(p => p.checkins)
                //     .ThenInclude(c => c.Hospedagem)
                .Where(p => p.Situacao != Domain.Enums.Situacao.Livre &&
                            p.checkins != null &&
                            p.checkins.Hospedagem.Any(h => h.PrevisaoFechamento.Date < dataAtual))
                .ToListAsync();

            return apartamentos;
        }

        public async Task<(List<QuartoStatusDto> QuantidadesPorStatus, int Total)> ObterQuantidadeQuartosPorStatusAsync()
        {
            var resultado = await _context.Apartamentos
                .GroupBy(q => q.Situacao)
                .Select(grupo => new QuartoStatusDto
                {
                    Situacao = grupo.Key.ToString(),
                    Quantidade = grupo.Count()
                })
                .ToListAsync();

            int total = resultado.Sum(r => r.Quantidade);

            // Retorna a lista agrupada e o total
            return (resultado, total);
        }


        public async Task<IPaginatedList<Apartamentos>> GetFilteredApartamentoquery(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            var aux = await IPaginatedList<Apartamentos>.ToPagedList(
             _context.Apartamentos
                                 .Include(p => p.TipoApartamentos)
                                 .Include(p => p.TipoGovernancas)
                                 .Include(h => h.checkins).ThenInclude(h => h.Hospedes).ThenInclude(c => c.Clientes)

                                 .Include(h => h.checkins)//.ThenInclude(h => h.Hospedagem).ThenInclude(c => c.Empresas)
                                 .AsNoTracking()
                                 .Where(r => r.Codigo.Trim().ToLower().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ? paginationFilter.FieldFilter.ToLower() : "")

                                 )
           //      .ToListAsync();
           , paginationFilter.PageNumber, paginationFilter.PageSize);

            return aux;
        }

        public IQueryable GetFilteredAsync(Domain.Interface.Shared.PaginationFilter paginationFilter)
        {
            IQueryable<Apartamentos> query = Enumerable.Empty<Apartamentos>().AsQueryable();
            query = (from apart in _context.Apartamentos
                                     .Include(p => p.TipoApartamentos)
                                     .Include(p => p.TipoGovernancas)
                                     //  .Include(h=>h.Hospedagems).ThenInclude(c=>c.Checkins)
                                     .Include(h => h.checkins).ThenInclude(h => h.Hospedes)
                                     .Include(h => h.checkins)//.ThenInclude(h => h.Pagamentos)
                                     .AsNoTracking()
                                     .Where(r => r.Codigo.Trim().ToLower().Contains(!string.IsNullOrWhiteSpace(paginationFilter.FieldFilter) ? paginationFilter.FieldFilter.ToLower() : ""))
                     select apart);
            return query;
        }


        /// <summary>
        /// Atualiza a situação de todos os apartamentos com base nas hospedagens ativas e datas de checkout.
        /// Lógica melhorada com validações e tratamento de casos especiais.
        /// </summary>
        public async Task AtualizarSituacaoApartamentosAsync()
        {
            try
            {
                var dataAtual = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time")).Date;

                // Recuperar TODOS os apartamentos (com e sem check-in)
                var apartamentos = await _context.Apartamentos
                    .Include(a => a.checkins)
                        .ThenInclude(c => c.Hospedagem)
                    .ToListAsync();

                var apartamentosAtualizados = 0;

                foreach (var apartamento in apartamentos)
                {
                    var situacaoAnterior = apartamento.Situacao;
                    var novaSituacao = DeterminarSituacaoApartamento(apartamento, dataAtual);

                    if (situacaoAnterior != novaSituacao)
                    {
                        apartamento.Situacao = novaSituacao;
                        apartamentosAtualizados++;

                        System.Diagnostics.Debug.WriteLine(
                            $"Apartamento {apartamento.Codigo}: {situacaoAnterior} → {novaSituacao}"
                        );
                    }
                }

                // Salvar apenas se houve mudanças
                if (apartamentosAtualizados > 0)
                {
                    await _context.SaveChangesAsync();
                    System.Diagnostics.Debug.WriteLine($"Atualizadas {apartamentosAtualizados} situações de apartamentos.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao atualizar situações: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Determina a situação correta de um apartamento baseada na hospedagem ativa e data atual.
        /// </summary>
        /// <param name="apartamento">Apartamento a ser analisado</param>
        /// <param name="dataAtual">Data atual para comparação</param>
        /// <returns>Situação apropriada para o apartamento</returns>
        private Situacao DeterminarSituacaoApartamento(Apartamentos apartamento, DateTime dataAtual)
        {
            var checkin = apartamento.checkins;

            // Se não há check-in ativo, apartamento está livre
            if (checkin == null || checkin.Hospedagem == null || !checkin.Hospedagem.Any())
            {
                return Situacao.Livre;
            }

            // Busca a hospedagem mais relevante (mais recente ou ativa)
            var hospedagemAtiva = checkin.Hospedagem
                .Where(h => h.PrevisaoFechamento != DateTime.MinValue) // Apenas hospedagens com data válida
                .OrderByDescending(h => h.DataAbertura) // Hospedagem mais recente primeiro
                .ThenByDescending(h => h.PrevisaoFechamento)
                .FirstOrDefault();

            // Se não há hospedagem com previsão de fechamento válida, considera livre
            if (hospedagemAtiva == null || hospedagemAtiva.PrevisaoFechamento == DateTime.MinValue)
            {
                return Situacao.Livre;
            }

            var previsaoFechamento = hospedagemAtiva.PrevisaoFechamento.Date;
            var diferenca = (previsaoFechamento - dataAtual).Days;

            // Lógica clara para determinar situação baseada na diferença de dias
            return diferenca switch
            {
                < 0 => Situacao.Atrasado,     // Checkout em atraso (data passou)
                0 => Situacao.Hoje,          // Checkout hoje
                1 => Situacao.Amanha,        // Checkout amanhã
                _ => Situacao.Ocupado        // Checkout em data futura (mais de 1 dia)
            };
        }
        /// <summary>
        /// Marca um apartamento como ocupado por um check-in específico.
        /// </summary>
        /// <param name="IdApartamento">ID do apartamento</param>
        /// <param name="CheckinsId">ID do check-in</param>
        public void ocuparApartamento(int IdApartamento, int CheckinsId)
        {
            var apartamento = _context.Apartamentos.Find(IdApartamento);
            if (apartamento == null) throw new Exception("Apartamento não encontrado.");

            if (apartamento.Situacao == Domain.Enums.Situacao.Ocupado) throw new Exception("Apartamento encontra-se ocupado.");
            apartamento.ocuparApartamento(CheckinsId);
            _context.Update(apartamento);
            // _context.SaveChanges();
        }

        public void desocuparApartamento(int IdApartamento)
        {
            var apartamento = _context.Apartamentos.Find(IdApartamento);
            if (apartamento == null) throw new Exception("Apartamento não encontrado.");
            apartamento.liberarApartamento();
            _context.Update(apartamento);
            // _context.SaveChanges();
        }
        public bool ApartamentoOcupado(int IdApartamento)
        {
            var apartamento = _context.Apartamentos.Find(IdApartamento);
            if (apartamento == null) throw new Exception("Apartamento não encontrado.");
            if (apartamento.Situacao == Domain.Enums.Situacao.Ocupado)
            {
                return true;
            }
            else { return false; }
            //  return apartamento.ocupado;
        }

        /// <summary>
        /// Verifica e corrige apartamentos com situação inválida no banco de dados.
        /// Útil para corrigir dados corrompidos ou inconsistentes.
        /// </summary>
        /// <returns>Número de apartamentos corrigidos</returns>
        public async Task<int> VerificarECorrigirSituacoesInvalidasAsync()
        {
            try
            {
                var apartamentosCorrigidos = 0;

                // Busca todos os apartamentos e verifica no código
                var todosApartamentos = await _context.Apartamentos
                    .Include(a => a.checkins)
                    .ToListAsync();

                foreach (var apartamento in todosApartamentos)
                {
                    var situacaoOriginal = apartamento.Situacao;

                    // Força a validação através do setter customizado
                    // Se a situação atual for inválida, será automaticamente corrigida para Livre
                    var situacaoTemporaria = apartamento.Situacao;
                    apartamento.Situacao = situacaoTemporaria;

                    // Verifica se houve mudança (indicando que havia valor inválido)
                    if (!situacaoOriginal.Equals(apartamento.Situacao))
                    {
                        // Aplica lógica de negócio para determinar situação correta
                        if (apartamento.CheckinsId.HasValue && apartamento.CheckinsId > 0)
                        {
                            apartamento.Situacao = Situacao.Ocupado;
                        }
                        else
                        {
                            apartamento.Situacao = Situacao.Livre;
                        }

                        System.Diagnostics.Debug.WriteLine(
                            $"Apartamento {apartamento.Codigo}: Situação corrigida de valor inválido para '{apartamento.Situacao}'"
                        );

                        apartamentosCorrigidos++;
                    }
                }

                if (apartamentosCorrigidos > 0)
                {
                    await _context.SaveChangesAsync();
                    System.Diagnostics.Debug.WriteLine($"Total de {apartamentosCorrigidos} apartamentos com situação corrigida.");
                }

                return apartamentosCorrigidos;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao verificar situações inválidas: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Atualiza a situação de apartamentos específicos baseado em uma lista de IDs.
        /// Útil para atualizações pontuais sem processar todo o banco.
        /// </summary>
        /// <param name="apartamentosIds">Lista de IDs dos apartamentos a serem atualizados</param>
        /// <returns>Número de apartamentos atualizados</returns>
        public async Task<int> AtualizarSituacaoApartamentosEspecificosAsync(List<int> apartamentosIds)
        {
            if (apartamentosIds == null || !apartamentosIds.Any())
                return 0;

            try
            {
                var dataAtual = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                    TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time")).Date;

                var apartamentos = await _context.Apartamentos
                    .Include(a => a.checkins)
                        .ThenInclude(c => c.Hospedagem)
                    .Where(a => apartamentosIds.Contains(a.Id))
                    .ToListAsync();

                var apartamentosAtualizados = 0;

                foreach (var apartamento in apartamentos)
                {
                    var situacaoAnterior = apartamento.Situacao;
                    var novaSituacao = DeterminarSituacaoApartamento(apartamento, dataAtual);

                    if (situacaoAnterior != novaSituacao)
                    {
                        apartamento.Situacao = novaSituacao;
                        apartamentosAtualizados++;
                    }
                }

                if (apartamentosAtualizados > 0)
                {
                    await _context.SaveChangesAsync();
                }

                return apartamentosAtualizados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao atualizar apartamentos específicos: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Obtém um relatório das situações dos apartamentos para monitoramento.
        /// </summary>
        /// <returns>Relatório com contadores por situação</returns>
        public async Task<Dictionary<string, int>> ObterRelatorioSituacoesAsync()
        {
            try
            {
                var relatorio = await _context.Apartamentos
                    .GroupBy(a => a.Situacao)
                    .Select(g => new { Situacao = g.Key.ToString(), Quantidade = g.Count() })
                    .ToDictionaryAsync(x => x.Situacao, x => x.Quantidade);

                // Garantir que todas as situações apareçam no relatório, mesmo com 0
                var todasSituacoes = Enum.GetNames(typeof(Situacao));
                foreach (var situacao in todasSituacoes)
                {
                    if (!relatorio.ContainsKey(situacao))
                        relatorio[situacao] = 0;
                }

                return relatorio;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao gerar relatório: {ex.Message}");
                return new Dictionary<string, int>();
            }
        }

        /// <summary>
        /// Busca apartamentos que NÃO estão livres
        /// </summary>
        /// <returns>Lista de apartamentos não livres</returns>
        public async Task<List<Apartamentos>> GetApartamentosOcupadosAsync()
        {
            return await _context.Apartamentos
                             .Include(p => p.TipoApartamentos)
                             .Include(p => p.TipoGovernancas)
                             .Include(h => h.checkins)
                                 .ThenInclude(h => h.Hospedes)
                                 .ThenInclude(c => c.Clientes)
                             .Include(h => h.checkins)
                                 .ThenInclude(h => h.Hospedagem)
                                 .ThenInclude(c => c.Empresas)
                             .Include(h => h.checkins)
                                // .ThenInclude(h => h.Pagamentos)
                             .Where(p => p.Situacao != Situacao.Livre)
                             .OrderBy(p => p.Codigo)
                             .ToListAsync();
        }
public async Task<List<Apartamentos>> GetApartamentosOcupados2Async()
{
    var apartamentos = await _context.Apartamentos
        .Where(p => p.Situacao != Situacao.Livre && p.CheckinsId.HasValue)
        .Include(p => p.TipoApartamentos)
        .Include(p => p.TipoGovernancas)
        .Include(h => h.checkins)
            .ThenInclude(h => h.Hospedes)
            .ThenInclude(c => c.Clientes)
        .Include(h => h.checkins)
            .ThenInclude(h => h.Hospedagem)
            .ThenInclude(c => c.Empresas)
        .AsNoTracking()
        .OrderBy(p => p.Codigo)
        .ToListAsync();

    // Filtrar in-memory: manter apenas hospedagem aberta do checkin atual
    foreach (var apart in apartamentos)
    {
        var hospedagens = apart.checkins?.Hospedagem;
        if (hospedagens != null)
        {
            var filtradas = hospedagens
                .Where(h => h.DataFechamento == null && h.CheckinsId == apart.CheckinsId)
                .ToList();

            // Se for uma lista mutável (List<T> implementa IList), limpamos e repopulamos
            if (hospedagens is IList nonGenericList)
            {
                nonGenericList.Clear();
                foreach (var h in filtradas)
                    nonGenericList.Add(h);
            }
            else
            {
                // Tenta atribuir via setter apenas se o setter for acessível
                var prop = apart.checkins.GetType().GetProperty("Hospedagem");
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(apart.checkins, filtradas);
                }
                // Caso contrário, não é possível alterar a coleção; mantemos a original
            }
        }
    }

    return apartamentos;
}



         public async Task<List<Apartamentos>> GetApartamentosOcupadosSemIncludeAsync()
        {
            return await _context.Apartamentos
                            // .IgnoreQueryFilters()
                             .AsNoTracking()
                             .Where(p => p.CheckinsId.HasValue) // ← Verificar CheckinsId, não Situacao
                             
                             .OrderBy(p => p.Codigo)
                             .ToListAsync();
        }
        /// <summary>
/// Busca apartamentos com APENAS o checkin ativo (mais recente com hospedagem aberta)
/// Filtra in-memory para evitar carregar todos os checkouts históricos
/// </summary>
public async Task<List<ApartamentoComCheckinAtivoDto>> GetApartamentosComCheckinAtivoAsync()
{
    var apartamentos = await _context.Apartamentos
        .Include(a => a.TipoApartamentos)
        .AsNoTracking()
        .ToListAsync();

    var resultado = new List<ApartamentoComCheckinAtivoDto>();

    foreach (var apart in apartamentos)
    {
        // Buscar hospedagens abertas deste apartamento
        var hospedagensAbertas = await _context.Hospedagems
            .Include(h => h.Checkins)
                .ThenInclude(c => c.Hospedes)
                .ThenInclude(h => h.Clientes)
            .Include(h => h.Empresas)
            .Where(h => h.ApartamentosId == apart.Id && h.DataFechamento == null)
            .AsNoTracking()
            .ToListAsync();

        if (!hospedagensAbertas.Any())
            continue;

        // Pegar a hospedagem mais recente
        var hospedagemAtiva = hospedagensAbertas
            .OrderByDescending(h => h.DataAbertura)
            .First();

        var checkin = hospedagemAtiva.Checkins;
        if (checkin == null)
            continue;

        var hospede = checkin.Hospedes?.FirstOrDefault();
        if (hospede == null)
            continue;

        resultado.Add(new ApartamentoComCheckinAtivoDto
        {
            Apartamento = apart,
            Hospedagem = hospedagemAtiva,
            Checkin = checkin,
            Hospede = hospede,
            Empresa = hospedagemAtiva.Empresas
        });
    }

    return resultado;
}
    }
}