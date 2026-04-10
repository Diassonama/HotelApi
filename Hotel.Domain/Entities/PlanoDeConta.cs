using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class PlanoDeConta: BaseDomainEntity
	{
		public string Descricao { get; set; }
		
		public int ContasId { get; set; }
		public ICollection<Produto> Produtos { get; set; }
		public ICollection<LancamentoCaixa> LancamentoCaixas { get; set; }

		[ForeignKey("ContasId")]
		public virtual Conta Contas { get; set; }
	}
}