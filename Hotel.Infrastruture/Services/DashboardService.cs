using Hotel.Application.DTOs.Dashboard;
using Hotel.Application.Interfaces;
using Hotel.Domain.Enums;
using Hotel.Infrastruture.Persistence.Context;
using Hotel.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Infrastruture.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly GhotelDbContext _context;

        public DashboardService(GhotelDbContext context)
        {
            _context = context;
        }

        // ==============================================================================
        // MÉTRICAS PRINCIPAIS PARA CARDS
        // ==============================================================================
        public async Task<IEnumerable<DashboardMetricDto>> GetDashboardMetricsAsync()
        {
            var today = DateTime.Today;
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var metrics = new List<DashboardMetricDto>();

            // Check-ins hoje
            var checkinsHoje = await _context.Checkins
                .Where(c => c.DataEntrada.Date == today)
                .CountAsync();

            metrics.Add(new DashboardMetricDto
            {
                Metric = "checkins_hoje",
                Valor = checkinsHoje,
                Label = "check-ins hoje",
                Cor = "#28a745"
            });

            // Check-outs hoje
            var checkoutsHoje = await _context.Checkins
                .Where(c => c.DataSaida.HasValue && 
                           c.DataSaida.Value.Date == today && 
                           c.CheckoutRealizado)
                .CountAsync();

            metrics.Add(new DashboardMetricDto
            {
                Metric = "checkouts_hoje",
                Valor = checkoutsHoje,
                Label = "check-outs hoje",
                Cor = "#dc3545"
            });

            // Taxa de ocupação
            var totalApartamentos = await _context.Apartamentos
                .CountAsync();

            var apartamentosOcupados = await _context.Apartamentos
                .Where(a => a.Situacao != Situacao.Livre)
                .CountAsync();

            var taxaOcupacao = totalApartamentos > 0 
                ? (int)((apartamentosOcupados * 100.0) / totalApartamentos)
                : 0;

            metrics.Add(new DashboardMetricDto
            {
                Metric = "ocupacao_atual",
                Valor = taxaOcupacao,
                Label = "% ocupação",
                Cor = "#007bff"
            });

            // Receita do mês - usando LancamentoCaixa com TipoLancamento.E (Entrada) e ValorPago > 0
            var receitaMes = await _context.LancamentoCaixas
                .Where(lc => lc.DataHoraLancamento.Month == currentMonth &&
                           lc.DataHoraLancamento.Year == currentYear &&
                           lc.TipoLancamento == TipoLancamento.E &&
                           lc.ValorPago > 0)
                .SumAsync(lc => (decimal?)lc.ValorPago) ?? 0;

            metrics.Add(new DashboardMetricDto
            {
                Metric = "receita_mes",
                Valor = (int)receitaMes,
                Label = "receita mês (€)",
                Cor = "#ffc107"
            });

            return metrics;
        }

        // ==============================================================================
        // CHECK-INS DE HOJE
        // ==============================================================================
        public async Task<IEnumerable<CheckinTodayDto>> GetCheckinsToday()
        {
            var today = DateTime.Today;

            return await _context.Checkins
                .Where(c => c.DataEntrada.Date == today)
                .Join(_context.Apartamentos,
                      c => c.Id, a => a.CheckinsId,
                      (c, a) => new { Checkin = c, Apartamento = a })
                .Join(_context.TipoApartamentos,
                      ca => ca.Apartamento.TipoApartamentosId, ta => ta.Id,
                      (ca, ta) => new { ca.Checkin, ca.Apartamento, TipoApartamento = ta })
                .GroupJoin(_context.Utilizadores,
                          cata => cata.Checkin.IdUtilizadorCheckin, u => u.Id,
                          (cata, users) => new { cata.Checkin, cata.Apartamento, cata.TipoApartamento, Users = users })
                .SelectMany(x => x.Users.DefaultIfEmpty(),
                           (x, user) => new CheckinTodayDto
                           {
                               Id = x.Checkin.Id,
                               DataEntrada = x.Checkin.DataEntrada,
                               ApartamentoCodigo = x.Apartamento.Codigo,
                               TipoApartamento = x.TipoApartamento.Descricao,
                               ValorTotalDiaria = x.Checkin.ValorTotalDiaria,
                               ValorTotalFinal = x.Checkin.ValorTotalFinal,
                               CamaExtra = x.Checkin.CamaExtra,
                               Observacao = x.Checkin.Observacao,
                               UtilizadorCheckin = user != null ? user.UserName : ""
                           })
                .OrderByDescending(x => x.DataEntrada)
                .ToListAsync();
        }

        // ==============================================================================
        // CHECK-OUTS DE HOJE
        // ==============================================================================
        public async Task<IEnumerable<CheckoutTodayDto>> GetCheckoutsToday()
        {
            var today = DateTime.Today;

            return await _context.Checkins
                .Where(c => c.DataSaida.HasValue && 
                           c.DataSaida.Value.Date == today && 
                           c.CheckoutRealizado)
                .Join(_context.Apartamentos,
                      c => c.Id, a => a.CheckinsId,
                      (c, a) => new { Checkin = c, Apartamento = a })
                .Join(_context.TipoApartamentos,
                      ca => ca.Apartamento.TipoApartamentosId, ta => ta.Id,
                      (ca, ta) => new { ca.Checkin, ca.Apartamento, TipoApartamento = ta })
                .GroupJoin(_context.Utilizadores,
                          cata => cata.Checkin.IdUtilizadorCheckOut, u => u.Id,
                          (cata, users) => new { cata.Checkin, cata.Apartamento, cata.TipoApartamento, Users = users })
                .SelectMany(x => x.Users.DefaultIfEmpty(),
                           (x, user) => new CheckoutTodayDto
                           {
                               Id = x.Checkin.Id,
                               DataSaida = x.Checkin.DataSaida.Value,
                               DataEntrada = x.Checkin.DataEntrada,
                               ApartamentoCodigo = x.Apartamento.Codigo,
                               TipoApartamento = x.TipoApartamento.Descricao,
                               ValorTotalFinal = x.Checkin.ValorTotalFinal,
                               ValorDesconto = x.Checkin.ValorDesconto,
                               PercentualDesconto = x.Checkin.PercentualDesconto,
                               CheckoutRealizado = x.Checkin.CheckoutRealizado,
                               UtilizadorCheckout = user != null ? user.UserName : "",
                               DiasHospedagem = x.Checkin.DataSaida.HasValue 
                                   ? (x.Checkin.DataSaida.Value.Date - x.Checkin.DataEntrada.Date).Days
                                   : 0
                           })
                .OrderByDescending(x => x.DataSaida)
                .ToListAsync();
        }

        // ==============================================================================
        // DADOS SEMANAIS PARA GRÁFICO
        // ==============================================================================
        public async Task<IEnumerable<WeeklyDataDto>> GetWeeklyDataAsync()
        {
            var today = DateTime.Today;
            var startWeek1 = today.AddDays(-(int)today.DayOfWeek).AddDays(-21); // 3 semanas atrás
            var startWeek2 = startWeek1.AddDays(7);
            var startWeek3 = startWeek1.AddDays(14);
            var startWeek4 = startWeek1.AddDays(21);
            var endDate = startWeek4.AddDays(7);

            // Criar lista de semanas
            var weeks = new List<WeeklyDataDto>
            {
                new WeeklyDataDto { SemanaLabel = "Semana 1", SemanaOrdem = 1, TotalCheckins = 0, TotalCheckouts = 0, ReceitaCheckins = 0, ReceitaCheckouts = 0 },
                new WeeklyDataDto { SemanaLabel = "Semana 2", SemanaOrdem = 2, TotalCheckins = 0, TotalCheckouts = 0, ReceitaCheckins = 0, ReceitaCheckouts = 0 },
                new WeeklyDataDto { SemanaLabel = "Semana 3", SemanaOrdem = 3, TotalCheckins = 0, TotalCheckouts = 0, ReceitaCheckins = 0, ReceitaCheckouts = 0 },
                new WeeklyDataDto { SemanaLabel = "Semana 4", SemanaOrdem = 4, TotalCheckins = 0, TotalCheckouts = 0, ReceitaCheckins = 0, ReceitaCheckouts = 0 }
            };

            // Check-ins por semana
            var checkins = await _context.Checkins
                .Where(c => c.DataEntrada >= startWeek1 && c.DataEntrada < endDate)
                .Select(c => new { c.DataEntrada, c.ValorTotalFinal })
                .ToListAsync();

            foreach (var checkin in checkins)
            {
                int weekIndex = 0;
                if (checkin.DataEntrada >= startWeek4) weekIndex = 3;
                else if (checkin.DataEntrada >= startWeek3) weekIndex = 2;
                else if (checkin.DataEntrada >= startWeek2) weekIndex = 1;
                else if (checkin.DataEntrada >= startWeek1) weekIndex = 0;

                if (weekIndex >= 0 && weekIndex < weeks.Count)
                {
                    weeks[weekIndex].TotalCheckins++;
                    weeks[weekIndex].ReceitaCheckins += (decimal)checkin.ValorTotalFinal;
                }
            }

            // Check-outs por semana
            var checkouts = await _context.Checkins
                .Where(c => c.DataSaida.HasValue && 
                           c.DataSaida >= startWeek1 && 
                           c.DataSaida < endDate && 
                           c.CheckoutRealizado)
                .Select(c => new { DataSaida = c.DataSaida.Value, c.ValorTotalFinal })
                .ToListAsync();

            foreach (var checkout in checkouts)
            {
                int weekIndex = 0;
                if (checkout.DataSaida >= startWeek4) weekIndex = 3;
                else if (checkout.DataSaida >= startWeek3) weekIndex = 2;
                else if (checkout.DataSaida >= startWeek2) weekIndex = 1;
                else if (checkout.DataSaida >= startWeek1) weekIndex = 0;

                if (weekIndex >= 0 && weekIndex < weeks.Count)
                {
                    weeks[weekIndex].TotalCheckouts++;
                    weeks[weekIndex].ReceitaCheckouts += (decimal)checkout.ValorTotalFinal;
                }
            }

            return weeks.OrderBy(w => w.SemanaOrdem);
        }

        // ==============================================================================
        // DADOS DE OCUPAÇÃO PARA GRÁFICO DOUGHNUT
        // ==============================================================================
        public async Task<IEnumerable<OccupancyDataDto>> GetOccupancyDataAsync()
        {
            var totalApartamentos = await _context.Apartamentos.CountAsync();
            var apartamentosOcupados = await _context.Apartamentos.Where(a => a.Situacao != Situacao.Livre).CountAsync();
            var apartamentosLivres = totalApartamentos - apartamentosOcupados;

            var result = new List<OccupancyDataDto>();

            if (apartamentosOcupados > 0)
            {
                result.Add(new OccupancyDataDto
                {
                    Label = "Ocupados",
                    Valor = apartamentosOcupados,
                    Percentual = totalApartamentos > 0 ? Math.Round((decimal)(apartamentosOcupados * 100.0 / totalApartamentos), 2) : 0,
                    Cor = "#28a745"
                });
            }

            if (apartamentosLivres > 0)
            {
                result.Add(new OccupancyDataDto
                {
                    Label = "Disponíveis",
                    Valor = apartamentosLivres,
                    Percentual = totalApartamentos > 0 ? Math.Round((decimal)(apartamentosLivres * 100.0 / totalApartamentos), 2) : 0,
                    Cor = "#e9ecef"
                });
            }

            return result.OrderByDescending(x => x.Valor);
        }

        // ==============================================================================
        // RECEITA ANUAL PARA GRÁFICO DE LINHA
        // ==============================================================================
        public async Task<IEnumerable<YearlyRevenueDto>> GetYearlyRevenueAsync()
        {
            var currentYear = DateTime.Now.Year;
            var currentMonth = DateTime.Now.Month;

            var months = new[]
            {
                new { Number = 1, Name = "Jan" }, new { Number = 2, Name = "Fev" },
                new { Number = 3, Name = "Mar" }, new { Number = 4, Name = "Abr" },
                new { Number = 5, Name = "Mai" }, new { Number = 6, Name = "Jun" },
                new { Number = 7, Name = "Jul" }, new { Number = 8, Name = "Ago" },
                new { Number = 9, Name = "Set" }, new { Number = 10, Name = "Out" },
                new { Number = 11, Name = "Nov" }, new { Number = 12, Name = "Dez" }
            };

            var revenueData = await _context.LancamentoCaixas
                .Where(lc => lc.DataHoraLancamento.Year == currentYear &&
                           lc.TipoLancamento == TipoLancamento.E &&
                           lc.ValorPago > 0)
                .GroupBy(lc => lc.DataHoraLancamento.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Revenue = g.Sum(lc => (decimal)lc.ValorPago),
                    Count = g.Count()
                })
                .ToListAsync();

            return months
                .Where(m => m.Number <= currentMonth)
                .Select(m =>
                {
                    var data = revenueData.FirstOrDefault(r => r.Month == m.Number);
                    return new YearlyRevenueDto
                    {
                        Label = m.Name,
                        MesNumero = m.Number,
                        Receita = data?.Revenue ?? 0,
                        TotalCheckouts = data?.Count ?? 0,
                        CorLinha = "#007bff",
                        CorFundo = "rgba(0, 123, 255, 0.1)"
                    };
                })
                .OrderBy(x => x.MesNumero);
        }

        // ==============================================================================
        // RECEITA DO MÊS ATUAL
        // ==============================================================================
        public async Task<MonthlyRevenueDto> GetMonthlyRevenueAsync()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var monthlyData = await _context.LancamentoCaixas
                .Where(lc => lc.DataHoraLancamento.Month == currentMonth &&
                           lc.DataHoraLancamento.Year == currentYear &&
                           lc.TipoLancamento == TipoLancamento.E &&
                           lc.ValorPago > 0)
                .GroupBy(lc => 1)
                .Select(g => new MonthlyRevenueDto
                {
                    Mes = currentMonth,
                    Ano = currentYear,
                    TotalCheckouts = g.Count(),
                    ReceitaTotal = g.Sum(lc => (decimal)lc.ValorPago),
                    ReceitaDiarias = g.Sum(lc => (decimal)lc.ValorPago), // Assumindo que todos são diárias
                    ReceitaConsumo = 0, // Não disponível em LancamentoCaixa
                    ReceitaLigacoes = 0, // Não disponível em LancamentoCaixa
                    TotalDescontos = 0, // Não disponível em LancamentoCaixa
                    TicketMedio = g.Average(lc => (decimal)lc.ValorPago),
                    MediaDiasHospedagem = 0 // Não disponível em LancamentoCaixa
                })
                .FirstOrDefaultAsync();

            return monthlyData ?? new MonthlyRevenueDto 
            { 
                Mes = currentMonth, 
                Ano = currentYear,
                TotalCheckouts = 0,
                ReceitaTotal = 0,
                ReceitaDiarias = 0,
                ReceitaConsumo = 0,
                ReceitaLigacoes = 0,
                TotalDescontos = 0,
                TicketMedio = 0,
                MediaDiasHospedagem = 0
            };
        }

        // ==============================================================================
        // ESTATÍSTICAS GERAIS DO DASHBOARD
        // ==============================================================================
        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            var today = DateTime.Today;

            var stats = new DashboardStatsDto();

            // Estatísticas de hoje
            stats.CheckinsHoje = await _context.Checkins
                .Where(c => c.DataEntrada.Date == today)
                .CountAsync();

            stats.CheckoutsHoje = await _context.Checkins
                .Where(c => c.DataSaida.HasValue && 
                           c.DataSaida.Value.Date == today && 
                           c.CheckoutRealizado)
                .CountAsync();

            stats.HospedesAtivos = await _context.Checkins
                .Where(c => !c.CheckoutRealizado)
                .CountAsync();

            // Estatísticas de apartamentos
            var apartamentos = await _context.Apartamentos
                .GroupBy(a => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Livres = g.Count(a => a.Situacao == Situacao.Livre),
                    Ocupados = g.Count(a => a.Situacao != Situacao.Livre),
                    Manutencao = g.Count(a => a.Situacao == Situacao.Manuntencao),
                    Limpeza = g.Count(a => a.Situacao == Situacao.Limpeza)
                })
                .FirstOrDefaultAsync();

            if (apartamentos != null)
            {
                stats.TotalApartamentos = apartamentos.Total;
                stats.ApartamentosLivres = apartamentos.Livres;
                stats.ApartamentosOcupados = apartamentos.Ocupados;
                stats.ApartamentosManutencao = apartamentos.Manutencao;
                stats.ApartamentosLimpeza = apartamentos.Limpeza;
                stats.TaxaOcupacao = apartamentos.Total > 0 
                    ? Math.Round((decimal)(apartamentos.Ocupados * 100.0 / apartamentos.Total), 2)
                    : 0;
            }

            return stats;
        }

        // ==============================================================================
        // DISTRIBUIÇÃO DE APARTAMENTOS
        // ==============================================================================
        public async Task<IEnumerable<ApartmentDistributionDto>> GetApartmentDistributionAsync()
        {
            return await _context.Apartamentos
                .Join(_context.TipoApartamentos,
                      a => a.TipoApartamentosId, ta => ta.Id,
                      (a, ta) => new { Apartamento = a, TipoApartamento = ta })
                .GroupJoin(_context.TipoGovernancas,
                          ata => ata.Apartamento.TipoGovernancasId, tg => tg.Id,
                          (ata, governancas) => new { ata.Apartamento, ata.TipoApartamento, Governancas = governancas })
                .SelectMany(x => x.Governancas.DefaultIfEmpty(),
                           (x, governanca) => new { x.Apartamento, x.TipoApartamento, Governanca = governanca })
                .GroupJoin(_context.Checkins,
                          x => x.Apartamento.CheckinsId, c => c.Id,
                          (x, checkins) => new { x.Apartamento, x.TipoApartamento, x.Governanca, Checkins = checkins })
                .SelectMany(x => x.Checkins.DefaultIfEmpty(),
                           (x, checkin) => new ApartmentDistributionDto
                           {
                               Id = x.Apartamento.Id,
                               Codigo = x.Apartamento.Codigo,
                               TipoApartamento = x.TipoApartamento.Descricao,
                               Capacidade = 1, // TipoApartamento entity doesn't have Capacidade property
                               ValorDiaria = (decimal?)x.TipoApartamento.ValorDiariaSingle,
                               SituacaoDescricao = x.Apartamento.Situacao == Situacao.Livre ? "Livre" :
                                                  x.Apartamento.Situacao == Situacao.Ocupado ? "Ocupado" :
                                                  x.Apartamento.Situacao == Situacao.Manuntencao ? "Manutenção" :
                                                  x.Apartamento.Situacao == Situacao.Atrasado ? "Atrasado" :
                                                  x.Apartamento.Situacao == Situacao.Hoje ? "Hoje" :
                                                  x.Apartamento.Situacao == Situacao.Amanha ? "Amanhã" :
                                                  x.Apartamento.Situacao == Situacao.Limpeza ? "Limpeza" :
                                                  x.Apartamento.Situacao == Situacao.Bloqueado ? "Bloqueado" : "Indefinido",
                               Situacao = (int)x.Apartamento.Situacao,
                               CodigoRamal = x.Apartamento.CodigoRamal,
                               CafeDaManha = x.Apartamento.CafeDaManha,
                               NaoPertube = x.Apartamento.NaoPertube,
                               TipoGovernanca = x.Governanca != null ? x.Governanca.Descricao : "",
                               DataCheckin = checkin != null ? checkin.DataEntrada : null,
                               DataCheckout = checkin != null ? checkin.DataSaida : null
                           })
                .OrderBy(x => x.Codigo)
                .ToListAsync();
        }

        // ==============================================================================
        // TOP APARTAMENTOS MAIS UTILIZADOS
        // ==============================================================================
        public async Task<IEnumerable<TopApartmentDto>> GetTopApartmentsAsync(int count = 10)
        {
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);

            return await _context.Checkins
                .Where(c => c.DataEntrada >= sixMonthsAgo)
                .Join(_context.Apartamentos,
                      c => c.Id, a => a.CheckinsId,
                      (c, a) => new { Checkin = c, Apartamento = a })
                .Join(_context.TipoApartamentos,
                      ca => ca.Apartamento.TipoApartamentosId, ta => ta.Id,
                      (ca, ta) => new { ca.Checkin, ca.Apartamento, TipoApartamento = ta })
                .GroupBy(x => new { x.Apartamento.Codigo, x.TipoApartamento.Descricao })
                .Select(g => new TopApartmentDto
                {
                    ApartamentoCodigo = g.Key.Codigo,
                    TipoApartamento = g.Key.Descricao,
                    TotalReservas = g.Count(),
                    ReceitaTotal = g.Sum(x => (decimal)x.Checkin.ValorTotalFinal),
                    TicketMedio = g.Average(x => (decimal)x.Checkin.ValorTotalFinal),
                    MediaDiasOcupacao = g.Average(x => x.Checkin.DataSaida.HasValue 
                        ? EF.Functions.DateDiffDay(x.Checkin.DataEntrada, x.Checkin.DataSaida.Value)
                        : 0)
                })
                .OrderByDescending(x => x.TotalReservas)
                .ThenByDescending(x => x.ReceitaTotal)
                .Take(count)
                .ToListAsync();
        }

        // ==============================================================================
        // CHECK-OUTS PRÓXIMOS
        // ==============================================================================
        public async Task<IEnumerable<UpcomingCheckoutDto>> GetUpcomingCheckoutsAsync(int days = 3)
        {
            var today = DateTime.Today;
            var futureDate = today.AddDays(days);

            return await _context.Checkins
                .Where(c => c.DataSaida.HasValue &&
                           c.DataSaida >= today &&
                           c.DataSaida <= futureDate &&
                           !c.CheckoutRealizado)
                .Join(_context.Apartamentos,
                      c => c.Id, a => a.CheckinsId,
                      (c, a) => new { Checkin = c, Apartamento = a })
                .Join(_context.TipoApartamentos,
                      ca => ca.Apartamento.TipoApartamentosId, ta => ta.Id,
                      (ca, ta) => new UpcomingCheckoutDto
                      {
                          Id = ca.Checkin.Id,
                          DataSaida = ca.Checkin.DataSaida,
                          ApartamentoCodigo = ca.Apartamento.Codigo,
                          TipoApartamento = ta.Descricao,
                          ValorTotalFinal = (decimal)ca.Checkin.ValorTotalFinal,
                          CheckoutRealizado = ca.Checkin.CheckoutRealizado,
                          DiasRestantes = EF.Functions.DateDiffDay(today, ca.Checkin.DataSaida.Value),
                          TempoRestante = EF.Functions.DateDiffDay(today, ca.Checkin.DataSaida.Value) == 0 ? "Hoje" :
                                         EF.Functions.DateDiffDay(today, ca.Checkin.DataSaida.Value) == 1 ? "Amanhã" :
                                         EF.Functions.DateDiffDay(today, ca.Checkin.DataSaida.Value).ToString() + " dias"
                      })
                .OrderBy(x => x.DataSaida)
                .ToListAsync();
        }

        // ==============================================================================
        // RESERVAS PRÓXIMAS PARA LISTA
        // ==============================================================================
        public async Task<IEnumerable<UpcomingReservationDto>> GetUpcomingReservationsAsync(int count = 5)
        {
            var today = DateTime.Today;

            return await _context.Checkins
                .Where(c => !c.CheckoutRealizado &&
                           ((c.DataEntrada.Date >= today) ||
                            (c.DataSaida.HasValue && c.DataSaida.Value.Date >= today)))
                .Join(_context.Apartamentos,
                      c => c.Id, a => a.CheckinsId,
                      (c, a) => new { Checkin = c, Apartamento = a })
                .Join(_context.TipoApartamentos,
                      ca => ca.Apartamento.TipoApartamentosId, ta => ta.Id,
                      (ca, ta) => new UpcomingReservationDto
                      {
                          Id = ca.Checkin.Id,
                          Apartamento = ca.Apartamento.Codigo,
                          TipoApartamento = ta.Descricao,
                          DataEntrada = ca.Checkin.DataEntrada.ToString("dd/MM/yyyy HH:mm"),
                          DataSaida = ca.Checkin.DataSaida.HasValue ? ca.Checkin.DataSaida.Value.ToString("dd/MM/yyyy") : "",
                          ValorTotalFinal = (decimal)ca.Checkin.ValorTotalFinal,
                          Status = ca.Checkin.CheckoutRealizado ? "Check-out" :
                                  ca.Checkin.DataEntrada.Date == today ? "Check-in" :
                                  ca.Checkin.DataEntrada.Date > today ? "Confirmado" : "Em andamento",
                          StatusClass = ca.Checkin.CheckoutRealizado ? "badge-secondary" :
                                       ca.Checkin.DataEntrada.Date == today ? "badge-success" :
                                       ca.Checkin.DataEntrada.Date > today ? "badge-warning" : "badge-primary"
                      })
                .OrderBy(x => x.Status == "Em andamento" ? 0 : 1)
                .ThenBy(x => x.DataEntrada)
                .Take(count)
                .ToListAsync();
        }

        // ==============================================================================
        // APARTAMENTOS OCUPADOS - LISTA DETALHADA
        // ==============================================================================
        public async Task<IEnumerable<ApartamentoOcupadoDto>> GetApartamentosOcupadosAsync()
        {
            var today = DateTime.Today;

            // Primeiro, obter estatísticas gerais dos apartamentos
            var totalQuartos = await _context.Apartamentos.CountAsync();
            var quartosOcupados = await _context.Apartamentos.Where(a => a.Situacao != Situacao.Livre).CountAsync();
            var quartosLivres = totalQuartos - quartosOcupados;
            var perOcupado = totalQuartos > 0 ? Math.Round((decimal)(quartosOcupados * 100.0 / totalQuartos), 2) : 0;
            var perLivre = totalQuartos > 0 ? Math.Round((decimal)(quartosLivres * 100.0 / totalQuartos), 2) : 0;

            return await _context.Hospedagems
                .Join(_context.Checkins, h => h.CheckinsId, c => c.Id, (h, c) => new { Hospedagem = h, Checkin = c })
                .Join(_context.Hospedes, hc => hc.Checkin.Id, ho => ho.CheckinsId, (hc, ho) => new { hc.Hospedagem, hc.Checkin, Hospede = ho })
                .Join(_context.Apartamentos.Where(a => a.Situacao != Situacao.Livre), 
                      hcho => hcho.Checkin.Id, a => a.CheckinsId, 
                      (hcho, a) => new { hcho.Hospedagem, hcho.Checkin, hcho.Hospede, Apartamento = a })
                .Join(_context.Clientes, hchoa => hchoa.Hospede.ClientesId, cl => cl.Id, 
                      (hchoa, cl) => new { hchoa.Hospedagem, hchoa.Checkin, hchoa.Hospede, hchoa.Apartamento, Cliente = cl })
                .GroupJoin(_context.TipoApartamentos, 
                          hchoac => hchoac.Apartamento.TipoApartamentosId, ta => ta.Id, 
                          (hchoac, tipoApts) => new { hchoac.Hospedagem, hchoac.Checkin, hchoac.Hospede, hchoac.Apartamento, hchoac.Cliente, TipoApartamentos = tipoApts })
                .SelectMany(x => x.TipoApartamentos.DefaultIfEmpty(), 
                           (x, ta) => new { x.Hospedagem, x.Checkin, x.Hospede, x.Apartamento, x.Cliente, TipoApartamento = ta })
                .GroupJoin(_context.Empresas, 
                          xta => xta.Hospedagem.EmpresasId, e => e.Id, 
                          (xta, empresas) => new { xta.Hospedagem, xta.Checkin, xta.Hospede, xta.Apartamento, xta.Cliente, xta.TipoApartamento, Empresas = empresas })
                .SelectMany(x => x.Empresas.DefaultIfEmpty(), 
                           (x, e) => new { x.Hospedagem, x.Checkin, x.Hospede, x.Apartamento, x.Cliente, x.TipoApartamento, Empresa = e })
                .GroupJoin(_context.Paises, 
                          xe => xe.Cliente.PaisId, p => p.Id, 
                          (xe, paises) => new { xe.Hospedagem, xe.Checkin, xe.Hospede, xe.Apartamento, xe.Cliente, xe.TipoApartamento, xe.Empresa, Paises = paises })
                .SelectMany(x => x.Paises.DefaultIfEmpty(), 
                           (x, p) => new ApartamentoOcupadoDto
                           {
                               Id = x.Checkin.Id,
                               Codigo = x.Apartamento.Codigo,
                               Hospede = x.Cliente.Nome,
                               Tipo = x.TipoApartamento != null ? x.TipoApartamento.Descricao : "",
                               Adultos = x.Hospedagem.QuantidadeHomens + x.Hospedagem.QuantidadeMulheres,
                               QuantidadeCrianca = x.Hospedagem.QuantidadeCrianca,
                               DataAbertura = x.Hospedagem.DataAbertura.ToString("dd/MM/yyyy"),
                               PrevisaoFechamento = x.Hospedagem.PrevisaoFechamento.ToString("dd/MM/yyyy"),
                               Pais = p != null ? p.Nome : "",
                               Empresa = x.Empresa != null ? x.Empresa.RazaoSocial : "",
                               Checkout = EF.Functions.DateDiffDay(today, x.Hospedagem.PrevisaoFechamento) == 0 ? "Hoje" :
                                         EF.Functions.DateDiffDay(today, x.Hospedagem.PrevisaoFechamento) == 1 ? "Amanha" :
                                         EF.Functions.DateDiffDay(today, x.Hospedagem.PrevisaoFechamento) < 0 ? "Check out Atrazado" :
                                         "Em " + EF.Functions.DateDiffDay(today, x.Hospedagem.PrevisaoFechamento).ToString() + " dia(s)",
                               ValorDiaria = (decimal)x.Hospedagem.ValorDiaria * x.Hospedagem.QuantidadeDeDiarias,

                               QuartoOcupado = quartosOcupados,
                               QuartoLivre = quartosLivres,
                               TotalQuarto = totalQuartos,
                               PerOcupado = perOcupado,
                               PerLivre = perLivre
                           })
                .OrderBy(x => x.Codigo)
                .ToListAsync();
        }
    }
}
