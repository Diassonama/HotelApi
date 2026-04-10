using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.Produto.Base
{
    public class ProdutoCommandBase : IRequest<BaseCommandResponse>
    {
        public string Nome { get; set; }
        public float Valor { get; set; }
        public int Quantidade { get; set; }
        public int EstoqueMinino { get; set; }
        public int AdicionarStock { get; set; }
        public float Lucro { get; set; }
        public float MargemLucro { get; set; }
        public DateTime DataExpiracao { get; set; }
        public float ValorFixo { get; set; }
        public float PrecoCompra { get; set; }
        public float Desconto { get; set; }
        public float DescontoPercentagem { get; set; }
        public int CategoriaId { get; set; }

        public int PontoDeVendasId { get; set; }
        public string CaminhoImagem { get; set; }

        public float PrecoCIva { get; set; }
        public string ProductTypeCode { get; set; }
        public int TaxTableEntryId { get; set; }
        public string TaxExemptionReasonCode { get; set; }
        public int EstoqueMinimo { get; set; }
    }
}