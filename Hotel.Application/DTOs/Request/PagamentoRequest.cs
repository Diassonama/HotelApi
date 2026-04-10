using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.DTOs.Request
{
    public class PagamentoRequest
    {
        public int CheckinsId  { get; set; }
        public int HospedesId { get; set; }
        public float Valor { get; set; }
        public float ValorPago { get; set; }
        public DateTime DataPagamento { get; set; }
        public int TipoPagamentosId  { get; set; }
        public string Observacao { get; set; }

    }
}