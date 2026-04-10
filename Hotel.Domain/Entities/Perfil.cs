using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Perfil: BaseDomainEntity
    {
        public Perfil()
		{
		}

		public string Descricao { get; set; }

		public ICollection<Acesso> Acessos { get; set; }
		//public ICollection<Utilizador> Utilizadores { get; set; }
    }
}