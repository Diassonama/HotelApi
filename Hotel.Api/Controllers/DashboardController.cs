using Hotel.Application.DTOs.Dashboard;
using Hotel.Application.Interfaces;
using Hotel.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
   // [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly IRackNotificationService _rackNotificationService;

        public DashboardController(
            IDashboardService dashboardService,
            IRackNotificationService rackNotificationService)
        {
            _dashboardService = dashboardService;
            _rackNotificationService = rackNotificationService;
        }

        /// <summary>
        /// Obtém métricas principais para os cards do dashboard
        /// </summary>
        [HttpGet("metrics")]
        public async Task<ActionResult<IEnumerable<DashboardMetricDto>>> GetDashboardMetrics()
        {
            try
            {
                var metrics = await _dashboardService.GetDashboardMetricsAsync();
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter métricas: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém check-ins de hoje
        /// </summary>
        [HttpGet("checkins-today")]
        public async Task<ActionResult<IEnumerable<CheckinTodayDto>>> GetCheckinsToday()
        {
            try
            {
                var checkins = await _dashboardService.GetCheckinsToday();
                return Ok(checkins);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter check-ins de hoje: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém check-outs de hoje
        /// </summary>
        [HttpGet("checkouts-today")]
        public async Task<ActionResult<IEnumerable<CheckoutTodayDto>>> GetCheckoutsToday()
        {
            try
            {
                var checkouts = await _dashboardService.GetCheckoutsToday();
                return Ok(checkouts);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter check-outs de hoje: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém dados semanais para gráfico (últimas 4 semanas)
        /// </summary>
        [HttpGet("weekly")]
        public async Task<ActionResult<IEnumerable<WeeklyDataDto>>> GetWeeklyData()
        {
            try
            {
                var weeklyData = await _dashboardService.GetWeeklyDataAsync();
                return Ok(weeklyData);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter dados semanais: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém dados de ocupação para gráfico doughnut
        /// </summary>
        [HttpGet("occupancy")]
        public async Task<ActionResult<IEnumerable<OccupancyDataDto>>> GetOccupancyData()
        {
            try
            {
                var occupancyData = await _dashboardService.GetOccupancyDataAsync();
                return Ok(occupancyData);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter dados de ocupação: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém dados de receita anual para gráfico de linha
        /// </summary>
        [HttpGet("revenue")]
        public async Task<ActionResult<IEnumerable<YearlyRevenueDto>>> GetYearlyRevenue()
        {
            try
            {
                var revenueData = await _dashboardService.GetYearlyRevenueAsync();
                return Ok(revenueData);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter dados de receita: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém receita do mês atual
        /// </summary>
        [HttpGet("monthly-revenue")]
        public async Task<ActionResult<MonthlyRevenueDto>> GetMonthlyRevenue()
        {
            try
            {
                var monthlyRevenue = await _dashboardService.GetMonthlyRevenueAsync();
                return Ok(monthlyRevenue);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter receita mensal: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém estatísticas gerais do dashboard
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
        {
            try
            {
                var stats = await _dashboardService.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter estatísticas: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém distribuição de apartamentos
        /// </summary>
        [HttpGet("apartments-distribution")]
        public async Task<ActionResult<IEnumerable<ApartmentDistributionDto>>> GetApartmentDistribution()
        {
            try
            {
                var distribution = await _dashboardService.GetApartmentDistributionAsync();
                return Ok(distribution);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter distribuição de apartamentos: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém top apartamentos mais utilizados
        /// </summary>
        [HttpGet("top-apartments")]
        public async Task<ActionResult<IEnumerable<TopApartmentDto>>> GetTopApartments([FromQuery] int count = 10)
        {
            try
            {
                var topApartments = await _dashboardService.GetTopApartmentsAsync(count);
                return Ok(topApartments);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter top apartamentos: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém check-outs próximos
        /// </summary>
        [HttpGet("upcoming-checkouts")]
        public async Task<ActionResult<IEnumerable<UpcomingCheckoutDto>>> GetUpcomingCheckouts([FromQuery] int days = 3)
        {
            try
            {
                var upcomingCheckouts = await _dashboardService.GetUpcomingCheckoutsAsync(days);
                return Ok(upcomingCheckouts);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter check-outs próximos: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtém reservas próximas para lista do dashboard
        /// </summary>
        [HttpGet("upcoming-reservations")]
        public async Task<ActionResult<IEnumerable<UpcomingReservationDto>>> GetUpcomingReservations([FromQuery] int count = 5)
        {
            try
            {
                var upcomingReservations = await _dashboardService.GetUpcomingReservationsAsync(count);
                return Ok(upcomingReservations);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter reservas próximas: {ex.Message}");
            }
        }

        /// <summary>
        /// Endpoint combinado para todas as métricas essenciais do dashboard
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetDashboardSummary()
        {
            try
            {
                var metrics = await _dashboardService.GetDashboardMetricsAsync();
                var weeklyData = await _dashboardService.GetWeeklyDataAsync();
                var occupancyData = await _dashboardService.GetOccupancyDataAsync();
                var upcomingReservations = await _dashboardService.GetUpcomingReservationsAsync(5);

                var summary = new
                {
                    Metrics = metrics,
                    WeeklyData = weeklyData,
                    OccupancyData = occupancyData,
                    UpcomingReservations = upcomingReservations
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao obter resumo do dashboard: {ex.Message}");
            }
        }

        /// <summary>
        /// Lista os apartamentos ocupados com detalhes dos hóspedes
        /// </summary>
        /// <returns>Lista de apartamentos ocupados</returns>
        [HttpGet("apartamentos-ocupados")]
        public async Task<IActionResult> GetApartamentosOcupados()
        {
            try
            {
                var data = await _dashboardService.GetApartamentosOcupadosAsync();
                
                // Notifica via SignalR sobre a atualização dos apartamentos ocupados
                await _rackNotificationService.NotifyApartamentosOcupadosAsync(data);
                
                return Ok(data);
            }
            catch (Exception ex)
            {
                await _rackNotificationService.NotifyErrorAsync("Erro ao obter apartamentos ocupados", ex.Message);
                return BadRequest($"Erro ao obter apartamentos ocupados: {ex.Message}");
            }
        }
    }
}
