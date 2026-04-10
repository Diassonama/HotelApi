using Hotel.Application.DTOs.Dashboard;

namespace Hotel.Application.Interfaces
{
    public interface IDashboardService
    {
        // Métricas principais para cards
        Task<IEnumerable<DashboardMetricDto>> GetDashboardMetricsAsync();
        
        // Check-ins e check-outs de hoje
        Task<IEnumerable<CheckinTodayDto>> GetCheckinsToday();
        Task<IEnumerable<CheckoutTodayDto>> GetCheckoutsToday();
        
        // Dados para gráficos
        Task<IEnumerable<WeeklyDataDto>> GetWeeklyDataAsync();
        Task<IEnumerable<OccupancyDataDto>> GetOccupancyDataAsync();
        Task<IEnumerable<YearlyRevenueDto>> GetYearlyRevenueAsync();
        
        // Receita e estatísticas
        Task<MonthlyRevenueDto> GetMonthlyRevenueAsync();
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        
        // Apartamentos e reservas
        Task<IEnumerable<ApartmentDistributionDto>> GetApartmentDistributionAsync();
        Task<IEnumerable<TopApartmentDto>> GetTopApartmentsAsync(int count = 10);
        Task<IEnumerable<UpcomingCheckoutDto>> GetUpcomingCheckoutsAsync(int days = 3);
        Task<IEnumerable<UpcomingReservationDto>> GetUpcomingReservationsAsync(int count = 5);
        Task<IEnumerable<ApartamentoOcupadoDto>> GetApartamentosOcupadosAsync();
    }
}
