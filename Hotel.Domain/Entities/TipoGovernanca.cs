using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class TipoGovernanca: BaseDomainEntity
	{
		public string Descricao { get; set; }
	//	public ICollection<TipoApartamento> TipoApartamentos { get; set; }

		public ICollection<Apartamentos> Apartamentos { get; set; }
		public ICollection<Governanca> Governancas { get; set; }

	}
}