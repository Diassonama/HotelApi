using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
public class TipoHospedagem:BaseDomainEntity
	{
		public TipoHospedagem()
		{
		}

		public string Descricao { get; set; }
		public float Valor { get; set; }

		public ICollection<Hospedagem> Hospedagens { get; set; }

		//public ICollection<Reserva> Reservas { get; set; }

	}
}