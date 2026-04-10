using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class TipoProduto: BaseDomainEntity
	{
		public string Nome { get; set; }
		public ICollection<Produto> Produtos { get; set; }
	}
}