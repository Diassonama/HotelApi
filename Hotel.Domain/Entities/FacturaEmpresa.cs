
using System.IO.Pipes;
using Hotel.Domain.Common;
using Hotel.Domain.Enums;

namespace Hotel.Domain.Entities
{
	public class FacturaEmpresa : BaseDomainEntity
	{
		public FacturaEmpresa()
		{
		}
		public int EmpresasId { get; private set; }
		public int NumeroFactura { get; private set; }
		public int CheckinsId { get; private set; } 
		public float Valor { get; private set; }
		public DateTime Data { get; private set; }
		public int Ano { get; private set; }
		public string Tipo { get; private set; }
		public SituacaoFactura SituacaoFacturas { get; private set; }
		public Empresa Empresas { get; private set; }
		public Checkins checkins {get; private set;}

		//  public ICollection<Checkins> Checkins { get; set; } = new List<Checkins>();

		public FacturaEmpresa(int checkinId, int numeroFactura, float valor, int empresasId, DateTime data, string tipo, int ano)
		{
			if (valor <= 0)
				throw new ArgumentException("O valor deve ser maior que zero.");

			NumeroFactura = numeroFactura;
			Valor = valor;
			EmpresasId = empresasId;
			Data = data;
			CheckinsId = checkinId;
			SituacaoFacturas = SituacaoFactura.Pendente;
			Tipo = tipo;
			Ano = ano;
			IsActive = true;
		}

		// Métodos de negócio
		public void Pagar()
		{
			if (SituacaoFacturas != SituacaoFactura.Pendente)
				throw new InvalidOperationException("Só é possível pagar faturas pendentes.");

			SituacaoFacturas = SituacaoFactura.Paga;
		}

		public void Cancelar()
		{
			if (SituacaoFacturas == SituacaoFactura.Paga)
				throw new InvalidOperationException("Não é possível cancelar uma fatura paga.");

			SituacaoFacturas = SituacaoFactura.Cancelada;
		}
	}

	/* public enum SituacaoFactura
	{
		Pendente,
		Paga,
		Cancelada
	} */
}
