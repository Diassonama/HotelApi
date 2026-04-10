using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class FacturaDividida: BaseDomainEntity
	{
		public FacturaDividida()
		{
		}
		public DateTime DataEntrada { get; set; }
		public DateTime DataSaida { get; set; }
		public float Valor { get; set; }
		public int Grupo { get; set; }
		public int IdHospedagem { get; set; }
		public DateTime DateTime { get; set; }

		//[ForeignKey("IdCheckin")]
		public Checkins Checkins { get; set; }
	//	[ForeignKey("IdHospedagem")]
		public Hospedagem Hospedagens { get; set; }

	}
}