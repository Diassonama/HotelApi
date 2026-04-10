using System;
using System.ComponentModel.DataAnnotations.Schema;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public enum EstadoConta { Pendente, Parcial, Paga, Cancelada }

    public class ContaReceber : BaseDomainEntity
    {
        public int EmpresaId { get; private set; }
        public int? CheckinsId { get; private set; }
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
        [ForeignKey("CheckinsId")]
        public Checkins Checkins { get; private set; }

        // Parameterless constructor for EF Core
        private ContaReceber() { }

        public ContaReceber(int empresaId, decimal valorTotal, DateTime dataEmissao, DateTime? vencimento, string documento, int? checkinsId = null, string observacao = null)
        {
            if (empresaId <= 0) throw new ArgumentException("Empresa inválida.");
            if (valorTotal <= 0) throw new ArgumentException("Valor deve ser positivo.");
            EmpresaId = empresaId;
            ValorTotal = valorTotal;
            DataEmissao = dataEmissao;
            DataVencimento = vencimento;
            Documento = documento ?? string.Empty;
            CheckinsId = checkinsId;
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