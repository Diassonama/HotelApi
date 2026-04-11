using System;
using System.Collections.Generic;

namespace Hotel.Application.DTOs
{
    public class ReciboPedidoDto
    {
        // ─── Dados do hotel ───────────────────────────────────────────────
        public string NomeHotel { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string NumContribuinte { get; set; }

        // ─── Dados do pedido ──────────────────────────────────────────────
        public string NumePedido { get; set; }
        public DateTime DataPedido { get; set; }
        public string PontoVendaNome { get; set; }
        public string Observacao { get; set; }

        // ─── Cliente / Hóspede ────────────────────────────────────────────
        public string NomeCliente { get; set; }       // nome do hóspede ou "Cliente Diverso"
        public string ApartamentoCodigo { get; set; } // null se cliente diverso

        // ─── Itens ────────────────────────────────────────────────────────
        public List<ReciboPedidoItemDto> Itens { get; set; } = new();

        // ─── Totais ───────────────────────────────────────────────────────
        public decimal ValorTotal { get; set; }
        public decimal ValorPago { get; set; }
        public string FormaPagamento { get; set; }
        public string Operador { get; set; }
    }

    public class ReciboPedidoItemDto
    {
        public string Descricao { get; set; }
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }
}
