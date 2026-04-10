using Hotel.Domain.Enums;

namespace Hotel.Application.DTOs.Pedido
{
    public class RelatorioPedidosDto
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public int TotalPedidos { get; set; }
        public decimal ReceitaTotal { get; set; }
        public decimal TicketMedio { get; set; }
        public int PedidosPagos { get; set; }
        public int PedidosPendentes { get; set; }
        public int PedidosCancelados { get; set; }
        public int PedidosEstornados { get; set; }

        public List<RelatorioPorPontoVendaDto> PorPontoVenda { get; set; } = new();
        public List<RelatorioPorDiaDto> PorDia { get; set; } = new();
        public List<ProdutoMaisVendidoDto> ProdutosMaisVendidos { get; set; } = new();
    }

    public class RelatorioPorPontoVendaDto
    {
        public int PontoVendaId { get; set; }
        public string NomePontoVenda { get; set; } = string.Empty;
        public int TotalPedidos { get; set; }
        public decimal Receita { get; set; }
        public decimal TicketMedio { get; set; }
    }

    public class RelatorioPorDiaDto
    {
        public DateTime Data { get; set; }
        public int TotalPedidos { get; set; }
        public decimal Receita { get; set; }
        public decimal TicketMedio { get; set; }
    }

    public class ProdutoMaisVendidoDto
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int QuantidadeVendida { get; set; }
        public decimal ReceitaTotal { get; set; }
        public decimal PrecoMedio { get; set; }
    }
}