using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Pagamento:BaseDomainEntity
    {
        public Pagamento()
        {
        }
        public float Valor { get; private set; }
        public DateTime DataVencimento { get; private  set; }
        public string Observacao { get;  private set; }
        public int HospedesId { get;  private set; }
        public string UtilizadoresId { get;  private set; }
        public string Origem { get; set; } // Ex: "Checkin", "ContaReceber", "ContaPagar", "Venda"
        public int OrigemId { get; set; } // ID da entidade de origem
      //  public Hospedagem Hospedagens { get; set; }//
        public StatusPagamento Status { get; private set; }

        public Hospede Hospedes { get; set; }
        public ICollection<LancamentoCaixa> LancamentoCaixas { get; set; }
        public Utilizador Utilizadores { get; set; }

    // Construtor atualizado - apenas Origem e OrigemId
    public Pagamento(float valor, DateTime dataVencimento, int hospedesId, string utilizadoresId, string origem, int origemId, string observacao = null)
    {
        SetValor(valor);
        SetDataVencimento(dataVencimento);
        HospedesId = hospedesId;
        DateCreated = DateTime.Now;
        IsActive = true;
        Origem = origem ?? throw new ArgumentNullException(nameof(origem));
        OrigemId = origemId;
        UtilizadoresId = utilizadoresId;
        Observacao = observacao;
       // Status = StatusPagamento.Pendente;
    }

     public void SetValor(float valor)
    {
        if (valor < 0)
            throw new ArgumentException("O valor do pagamento deve ser maior que zero.");
        
        Valor = valor;
    }

    public void SetDataVencimento(DateTime dataVencimento)
    {
        /* if (dataVencimento < DateTime.Now)
            throw new ArgumentException("A data de vencimento não pode ser no passado.");
 */        
        DataVencimento = dataVencimento;
    }

    public void SetObservacao(string observacao)
    {
        Observacao = observacao;
    }

public void AssociarHospede(Hospede hospede)
    {
        Hospedes = hospede ?? throw new ArgumentNullException(nameof(hospede));
        HospedesId = hospede.Id;
    }

    public void AdicionarLancamentoCaixa(LancamentoCaixa lancamentoCaixa)
    {
        LancamentoCaixas ??= new List<LancamentoCaixa>();
        LancamentoCaixas.Add(lancamentoCaixa);
    }
    
    public void ConfirmarPagamento()
    {
        if (Status != StatusPagamento.Pendente)
            throw new InvalidOperationException("O pagamento já foi processado.");

        Status = StatusPagamento.Pago;
    }

    public void CancelarPagamento()
    {
        if (Status == StatusPagamento.Pago)
            throw new InvalidOperationException("Não é possível cancelar um pagamento já confirmado.");

        Status = StatusPagamento.Cancelado;
    }

    public enum StatusPagamento { Pendente, Pago, Cancelado }
    }
}