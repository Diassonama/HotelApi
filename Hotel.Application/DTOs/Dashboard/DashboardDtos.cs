using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.Dashboard
{
    // ==============================================================================
    // DTOs PARA DASHBOARD
    // ==============================================================================

    public class DashboardMetricDto
    {
        public string Metric { get; set; }
        public int Valor { get; set; }
        public string Label { get; set; }
        public string Cor { get; set; }
    }

    public class CheckinTodayDto
    {
        public int Id { get; set; }
        public DateTime DataEntrada { get; set; }
        public string ApartamentoCodigo { get; set; }
        public string TipoApartamento { get; set; }
        public float ValorTotalDiaria { get; set; }
        public float ValorTotalFinal { get; set; }
        public int CamaExtra { get; set; }
        public string Observacao { get; set; }
        public string UtilizadorCheckin { get; set; }
    }

    public class CheckoutTodayDto
    {
        public int Id { get; set; }
        public DateTime? DataSaida { get; set; }
        public DateTime DataEntrada { get; set; }
        public string ApartamentoCodigo { get; set; }
        public string TipoApartamento { get; set; }
        public float ValorTotalFinal { get; set; }
        public float ValorDesconto { get; set; }
        public float PercentualDesconto { get; set; }
        public bool CheckoutRealizado { get; set; }
        public string UtilizadorCheckout { get; set; }
        public int DiasHospedagem { get; set; }
    }

    public class WeeklyDataDto
    {
        public string SemanaLabel { get; set; }
        public int SemanaOrdem { get; set; }
        public int TotalCheckins { get; set; }
        public int TotalCheckouts { get; set; }
        public decimal ReceitaCheckins { get; set; }
        public decimal ReceitaCheckouts { get; set; }
    }

    public class OccupancyDataDto
    {
        public string Label { get; set; }
        public int Valor { get; set; }
        public decimal Percentual { get; set; }
        public string Cor { get; set; }
    }

    public class MonthlyRevenueDto
    {
        public int Mes { get; set; }
        public int Ano { get; set; }
        public int TotalCheckouts { get; set; }
        public decimal ReceitaTotal { get; set; }
        public decimal ReceitaDiarias { get; set; }
        public decimal ReceitaConsumo { get; set; }
        public decimal ReceitaLigacoes { get; set; }
        public decimal TotalDescontos { get; set; }
        public decimal TicketMedio { get; set; }
        public double MediaDiasHospedagem { get; set; }
    }

    public class YearlyRevenueDto
    {
        public string Label { get; set; }
        public int MesNumero { get; set; }
        public decimal Receita { get; set; }
        public int TotalCheckouts { get; set; }
        public string CorLinha { get; set; }
        public string CorFundo { get; set; }
    }

    public class ApartmentDistributionDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public string TipoApartamento { get; set; }
        public int? Capacidade { get; set; }
        public decimal? ValorDiaria { get; set; }
        public string SituacaoDescricao { get; set; }
        public int Situacao { get; set; }
        public int CodigoRamal { get; set; }
        public int CafeDaManha { get; set; }
        public bool NaoPertube { get; set; }
        public string TipoGovernanca { get; set; }
        public DateTime? DataCheckin { get; set; }
        public DateTime? DataCheckout { get; set; }
    }

    public class DashboardStatsDto
    {
        public int CheckinsHoje { get; set; }
        public int CheckoutsHoje { get; set; }
        public int HospedesAtivos { get; set; }
        public int TotalApartamentos { get; set; }
        public int ApartamentosLivres { get; set; }
        public int ApartamentosOcupados { get; set; }
        public int ApartamentosManutencao { get; set; }
        public int ApartamentosLimpeza { get; set; }
        public decimal TaxaOcupacao { get; set; }
    }

    public class TopApartmentDto
    {
        public string ApartamentoCodigo { get; set; }
        public string TipoApartamento { get; set; }
        public int TotalReservas { get; set; }
        public decimal ReceitaTotal { get; set; }
        public decimal TicketMedio { get; set; }
        public double MediaDiasOcupacao { get; set; }
    }

    public class UpcomingCheckoutDto
    {
        public int Id { get; set; }
        public DateTime? DataSaida { get; set; }
        public string ApartamentoCodigo { get; set; }
        public string TipoApartamento { get; set; }
        public decimal ValorTotalFinal { get; set; }
        public bool CheckoutRealizado { get; set; }
        public int DiasRestantes { get; set; }
        public string TempoRestante { get; set; }
    }

    public class UpcomingReservationDto
    {
        public int Id { get; set; }
        public string Apartamento { get; set; }
        public string TipoApartamento { get; set; }
        public string DataEntrada { get; set; }
        public string DataSaida { get; set; }
        public decimal ValorTotalFinal { get; set; }
        public string Status { get; set; }
        public string StatusClass { get; set; }
    }
}
