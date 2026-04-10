using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class PagamentoBK:BaseDomainEntity
	{
		public PagamentoBK()
		{
		}
		public float Valor { get; private set; }
		public DateTime DataVencimento { get; private  set; }
		public string Observacao { get;  private set; }
		public int IdVenda { get;  private set; }
		public int HospedesId { get;  private set; }
		public int CheckinsId { get;  private set; }
		public string UtilizadoresId { get;  private set; }
        public string Origem { get; set; }
        public int OrigemId { get; set; }
		public Hospedagem Hospedagens { get; set; }
        public StatusPagamento Status { get; private set; }

	//	public TipoPagamento TipoPagamentos { get; set; }
		public Hospede Hospedes { get; set; }
        public Checkins checkins {get; set;}
		public ICollection<LancamentoCaixa> LancamentoCaixas { get; set; }
		public Utilizador Utilizadores { get; set; }



	/* public Pagamento(int idTenant, float valor, DateTime dataVencimento, string createdBy)
    {
        DateCreated = DateTime.Now;
        CreatedBy = createdBy;
        IdTenant = idTenant;
        Valor = valor;
        DataVencimento = dataVencimento;
        IsActive = true;
    } */
	public PagamentoBK(float valor, DateTime dataVencimento, int hospedesId, int checkinsId, string utilizadoresId, string Origem, int OrigemId)
    {
        SetValor(valor);
        SetDataVencimento(dataVencimento);
        IdVenda = 0;
        HospedesId = hospedesId;
        CheckinsId = checkinsId;
		DateCreated = DateTime.Now;
		IsActive = true;
        this.Origem = Origem;
        this.OrigemId = OrigemId;
		UtilizadoresId = utilizadoresId;
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
public void AssociarHospede(Hospede hospede)
    {
        Hospedes = hospede ?? throw new ArgumentNullException(nameof(hospede));
        HospedesId = hospede.Id;
    }

  /*   public void AssociarHospedagem(Hospedagem hospedagem)
    {
        Hospedagens = hospedagem ?? throw new ArgumentNullException(nameof(hospedagem));
        HospedagensId = hospedagem.Id;
    } */

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

    public enum StatusPagamento { Pendente, Pago, Cancelado }
	}
}