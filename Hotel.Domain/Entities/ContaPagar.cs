using System;
using System.ComponentModel.DataAnnotations.Schema;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class ContaPagar : BaseDomainEntity
    {
        public int? EmpresaId { get; private set; }
        public string FornecedorNome { get; private set; }
        public decimal ValorTotal { get; private set; }
        public decimal ValorPago { get; private set; }
        public decimal Saldo { get; private set; }
        public DateTime DataEmissao { get; private set; }
        public DateTime? DataVencimento { get; private set; }
        public string Documento { get; private set; }
        public string Observacao { get; private set; }
        public EstadoConta Estado { get; private set; }
        [ForeignKey("EmpresaId")]
        public Empresa Empresa { get; private set; }

        // Parameterless constructor for EF Core
        private ContaPagar() { }

        public ContaPagar(decimal valorTotal, DateTime dataEmissao, DateTime? vencimento, string documento, string fornecedorNome = null, int? empresaId = null, string observacao = null)
        {
            if (valorTotal <= 0) throw new ArgumentException("Valor deve ser positivo.");
            ValorTotal = valorTotal;
            DataEmissao = dataEmissao;
            DataVencimento = vencimento;
            Documento = documento ?? string.Empty;
            FornecedorNome = fornecedorNome ?? string.Empty;
            EmpresaId = empresaId;
            Observacao = observacao ?? string.Empty;
            Saldo = valorTotal;
            ValorPago = 0;
            Estado = EstadoConta.Pendente;
            IsActive = true;
            DateCreated = DateTime.Now;
        }

        public void RegistrarPagamento(decimal valor)
        {
            if (Estado == EstadoConta.Cancelada) throw new InvalidOperationException("Conta cancelada.");
            if (valor <= 0) throw new ArgumentException("Valor deve ser positivo.");
           // if (valor > Saldo) throw new InvalidOperationException("Pagamento excede o saldo.");

            ValorPago += valor;
            Saldo -= valor;

            Estado = Saldo == 0 ? EstadoConta.Paga : EstadoConta.Parcial;
            LastModifiedDate = DateTime.Now;
        }

        public void Cancelar()
        {
            if (Estado == EstadoConta.Paga) throw new InvalidOperationException("Não pode cancelar conta paga.");
            Estado = EstadoConta.Cancelada;
            LastModifiedDate = DateTime.Now;
        }
    }
}