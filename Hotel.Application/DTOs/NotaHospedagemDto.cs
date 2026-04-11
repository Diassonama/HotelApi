using System;
using System.Collections.Generic;

namespace Hotel.Application.Dtos
{
    public class NotaHospedagemDto
    {
        public string NomeHotel { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string NumContribuinte { get; set; }
        public string Telefone { get; set; }
        public string LogoCaminho { get; set; }
        public DateTime DataImpressao { get; set; }
        public int NumeroDocumento { get; set; }
        public string NomeHospede { get; set; }
        public string Empresa { get; set; }
        public string UtilizadorCheckin { get; set; }
        public string UtilizadorCheckout { get; set; }
        public string Operador { get; set; }
        public string Quarto { get; set; }
        public string TipoQuarto { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime DataSaida { get; set; }
        public int NumDias { get; set; }
        public float ValorDiaria { get; set; }
        public float ValorDiarias { get; set; }
        public float Consumo { get; set; }
        public float Desconto { get; set; }
        public float Total { get; set; }
        public float Pago { get; set; }
        public float APagar { get; set; }
        public List<NotaHospedagemPagamentoDto> Pagamentos { get; set; } = new();
        public List<NotaHospedagemHistoricoDto> Historicos { get; set; } = new();
        public List<NotaHospedagemPedidoDto> Pedidos { get; set; } = new();
    }

    public class NotaHospedagemPagamentoDto
    {
        public int Numero { get; set; }
        public string Tipo { get; set; }
        public DateTime Data { get; set; }
        public string Hospede { get; set; }
        public string Operador { get; set; }
        public float Valor { get; set; }
    }

    public class NotaHospedagemHistoricoDto
    {
        public int Numero { get; set; }
        public DateTime Data { get; set; }
        public string Observacao { get; set; }
        public string Operador { get; set; }
    }

    public class NotaHospedagemPedidoDto
    {
        public string NumePedido { get; set; }
        public DateTime DataPedido { get; set; }
        public string PontoVendaNome { get; set; }
        public List<NotaHospedagemPedidoItemDto> Itens { get; set; } = new();
        public float Total { get; set; }
    }

    public class NotaHospedagemPedidoItemDto
    {
        public string Descricao { get; set; }
        public int Quantidade { get; set; }
        public float PrecoUnitario { get; set; }
        public float Total { get; set; }
    }
}