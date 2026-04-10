using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Patrimonio: BaseDomainEntity
	{
		public Patrimonio()
		{
			
		}
		public string Descricao { get; set; }
		public ICollection<MobiliaTipoApartamento> MobiliaTipoApartamentos { get; set; }
		public ICollection<MobiliaApartamento> MobiliaApartamentos  { get; set; }
		
	}
}