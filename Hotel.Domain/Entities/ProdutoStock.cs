using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class ProdutoStock: BaseDomainEntity
	{
		public ProdutoStock()
		{
		}
        public int Quantidade { get; set; }
        public int QuantidadeMinima { get; set; } = 0;
        public int QuantidadeMaxima { get; set; } = int.MaxValue;
        public string? Observacoes { get; set; }
        
        // Foreign Key
        public int ProdutoId { get; set; }
        public Produtos Produto { get; set; }
    }
}