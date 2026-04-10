using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Lavandaria: BaseDomainEntity
    {
        public Lavandaria()
        {
            
        }
        public int CaixaAberto { get; set; }
		public DateTime DataEntrada { get; set; }
		public DateTime DataSaida { get; set; }
		public float Valor { get; set; }
		public string SituacaoPagamento { get; set; }
		public int RegistoPagamento { get; set; }

		public Utilizador Utilizadores { get; set; }
		public Cliente Clientes { get; set; }
		public float PercentagemDesconto { get; set; }
		public float ValorDesconto { get; set; }
		public string SituacaoServico { get; set; }
		public ICollection<LavandariaItem> LavandariaItems { get; set; }
    }
}