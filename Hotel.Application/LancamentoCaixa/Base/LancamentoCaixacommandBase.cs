using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.LancamentoCaixa.Base
{
    public class LancamentoCaixacommandBase : IRequest<BaseCommandResponse>
    {
        public int HospedagensId { get; set; }
        public int HospedesId { get; set; }
        public int CaixasId { get; set; }
        public float Valor { get; set; }
        public float ValorPago { get; set; }
        public DateTime DataPagamento { get; set; }
        public int TipoPagamentosId { get; set; }
        public int PagamentosId { get; set; }
        public string Observacao { get; set; }
        public int ReferenciaId { get; set; } // Pode ser zero se não houver referência
        public int PlanoDeContasId { get; set; } // Pode ser zero se
    }
}