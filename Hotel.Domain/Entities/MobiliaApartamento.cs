using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
   public class MobiliaApartamento:BaseDomainEntity
	{
		public MobiliaApartamento()
		{
			
		}
		public int Quantidade { get; set; }
		public Patrimonio Patrimonios { get; set; }
		public TipoApartamento TipoApartamentos { get; set; }
	}
}