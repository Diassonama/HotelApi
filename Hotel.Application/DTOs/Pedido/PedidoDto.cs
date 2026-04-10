using Hotel.Domain.Enums;

namespace Hotel.Application.DTOs.Pedido
{
    public class PedidoDto
    {
        public int Id { get; set; }
        public int IdCaixa { get; set; }
        public int IdCheckin { get; set; }
        public int HospedeId { get; set; }
        public int PontoVendaId { get; set; }
        public string NumePedido { get; set; } = string.Empty;
        public string? Observacao { get; set; }
        public DateTime DataPedido { get; set; }
        public DateTime? DataFinalizacao { get; set; }
        public SituacaoPagamentoPedido SituacaoPagamento { get; set; }
        public string SituacaoPagamentoDescricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal ValorTotal { get; set; }
        public int QuantidadeItens { get; set; }
        public bool PedidoFinalizado { get; set; }
        public bool PodeCancelar { get; set; }

        // Navigation Properties
        public string NomeHospede { get; set; } = string.Empty;
        public string PontoVendaNome { get; set; } = string.Empty;
        
        public List<ItemPedidoDto> Itens { get; set; } = new();
    }

    public class PedidoListDto
    {
        public int Id { get; set; }
        public string NumePedido { get; set; } = string.Empty;
        public string NomeHospede { get; set; } = string.Empty;
        public string PontoVendaNome { get; set; } = string.Empty;
        public DateTime DataPedido { get; set; }
        public SituacaoPagamentoPedido SituacaoPagamento { get; set; }
        public string SituacaoPagamentoDescricao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal ValorTotal { get; set; }
        public int QuantidadeItens { get; set; }
    }

    public class ItemPedidoDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
        public string? Observacao { get; set; }
        public string? Categoria { get; set; }
        public decimal ValorTotal { get; set; }
    }
}