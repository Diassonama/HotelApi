using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;
using Hotel.Domain.Enums;

namespace Hotel.Domain.Entities
{
	public class LancamentoCaixa : BaseDomainEntity
	{
		public LancamentoCaixa()
		{
		}

		public DateTime DataHoraLancamento { get; private set; }
		public DateTime DataHoraVencimento { get; private set; }
		public float Valor { get; private set; }
		public float ValorPago { get; private set; }
		public float Troco { get; private set; }
		public string Observacao { get; private set; }
		//	public TipoApartamento TipoApartamentos { get; set; }
		public int CaixasId { get; private set; }
		public int TipoPagamentosId { get; private set; }
		public int PagamentosId { get; private set; }
		public string UtilizadoresId { get; set; }
		public int PlanoDeContasId { get; private set; }
		public int ReferenciaId { get; private set; }
		public Caixa Caixas { get; set; }
		public TipoPagamento TipoPagamentos { get; set; }
		public Pagamento Pagamentos { get; set; }
		public Utilizador Utilizadores { get; set; }
		public TipoLancamento TipoLancamento { get; set; }
	

		public PlanoDeConta PlanodeContas { get; set; }

		public LancamentoCaixa(float valor, DateTime dataHoraLancamento, DateTime dataHoraVencimento, int tipoPagamentosId, int pagamentosId, int caixasId, TipoLancamento tipoLancamento, string observacao, int planoDeContasId, string utilizadoresId)
		{
			if (valor <= 0)
				throw new ArgumentException("O valor deve ser positivo.");
			if (caixasId <= 0)
				throw new ArgumentException("O caixa ainda não foi aberto. Queira por favor abrir o caixa");
			if (dataHoraVencimento < dataHoraLancamento)
				throw new ArgumentException("A data de vencimento deve ser posterior à data de lançamento.");

			Valor = valor;
			DataHoraLancamento = dataHoraLancamento;
			DataHoraVencimento = dataHoraVencimento;
			DateCreated = DateTime.Now;
			TipoPagamentosId = tipoPagamentosId;
			CaixasId = caixasId;
			PagamentosId = pagamentosId;
			TipoLancamento = tipoLancamento;
			LastModifiedDate = DateCreated;
			Observacao = observacao;
			ReferenciaId = pagamentosId; // Inicializa com zero, pode ser ajustado posteriormente
			PlanoDeContasId = planoDeContasId;
			UtilizadoresId = utilizadoresId;
		}

		// Método de negócio para calcular troco
		public void CalcularTroco()
		{
				Troco = ValorPago > Valor? ValorPago - Valor : 0;			
		}

		// Método para definir o valor pago e recalcular troco
		public void DefinirValorPago(float valorPago)
		{
			if (valorPago < 0)
				throw new ArgumentException("O valor pago não pode ser negativo.");

			ValorPago = valorPago;
			CalcularTroco();
			//	AtualizarUltimaModificacao();
		}

		// Método para validar a data de vencimento
		public bool ValidarVencimento()
		{
			return DataHoraVencimento >= DateTime.Now;
		}

		// Método para ativar o lançamento
		public void Ativar()
		{
			if (!IsActive)
			{
				IsActive = true;
				AtualizarUltimaModificacao();
			}
		}

		// Método para inativar o lançamento
		public void Inativar()
		{
			if (IsActive)
			{
				IsActive = false;
				AtualizarUltimaModificacao();
			}
		}
		private void AtualizarUltimaModificacao()
		{
			LastModifiedDate = DateTime.Now;
		}

		// Método para ajustar o valor do lançamento
		public void AjustarValor(float novoValor)
		{
			if (novoValor <= 0)
				throw new ArgumentException("O valor do lançamento deve ser positivo.");

			Valor = novoValor;
			AtualizarUltimaModificacao();
		}
	}
}