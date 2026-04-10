using System;
using Hotel.Domain.Entities;

namespace Hotel.Application.DTOs
{
    public class ContaPagarDto
    {
        public int Id { get; set; }
        public int? EmpresaId { get; set; }
        public string NomeEmpresa { get; set; }
        public string FornecedorNome { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorPago { get; set; }
        public decimal Saldo { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime? DataVencimento { get; set; }
        public string Documento { get; set; }
        public string Observacao { get; set; }
        public EstadoConta Estado { get; set; }
        public string EstadoDescricao => Estado.ToString();
        public bool Vencida => DataVencimento.HasValue && DataVencimento.Value < DateTime.Now && Estado != EstadoConta.Paga;
        public int DiasVencimento => DataVencimento.HasValue ? (DateTime.Now.Date - DataVencimento.Value.Date).Days : 0;
    }
}