using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Dtos
{
    public class PagamentoRequest
    {
        public int CheckinsId { get; set; }
        public int HospedesId { get; set; }
        public float Valor { get; set; }
        public float ValorPago { get; set; }
        public DateTime DataPagamento { get; set; }
        public int TipoPagamentosId { get; set; }
        public string Observacao { get; set; }
        public string Origem { get; set; }
        public int OrigemId { get; set; }

    }
    
    public class PagamentoValorTotalResponse
    {
        public int CheckinId { get; set; }
        public float ValorTotalPago { get; set; }
        public int TotalPagamentos { get; set; }
        public DateTime? UltimoPagamento { get; set; }
        
        public PagamentoValorTotalResponse() { }
        
        public PagamentoValorTotalResponse(int checkinId, float valorTotalPago, int totalPagamentos, DateTime? ultimoPagamento = null)
        {
            CheckinId = checkinId;
            ValorTotalPago = valorTotalPago;
            TotalPagamentos = totalPagamentos;
            UltimoPagamento = ultimoPagamento;
        }
    }
}