using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Governanca: BaseDomainEntity
	{
		public Governanca()
		{
		}

		public int ApartamentosId { get; set; }
		public int TipoGovernancasId { get; set; }
		public DateTime DataInicio { get; set; }
		public DateTime DataTermino { get; set; }
		public string NomeDoResponsavel { get; set; }
		public string Observacao { get; set; }
		public Apartamentos Apartamentos { get; set; }
		public TipoGovernanca TipoGovernancas { get; set; }
	}
}