using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;
using Hotel.Domain.Identity;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Domain.Entities
{
	public class Utilizador : ApplicationUser
	{
		public Utilizador()
		{
		}


		public ICollection<Lavandaria> Lavandarias { get; set; }
		public ICollection<LavandariaItem> LavandariaItems { get; set; }
		public ICollection<Historico> Historicos { get; set; }
		public ICollection<Caixa> Caixas { get; set; }
		public ICollection<Reserva> Reservas { get; set; }
		public ICollection<Pagamento> Pagamentos { get; set; }
		public ICollection<LancamentoCaixa> LancamentoCaixas { get; set; }
		public ICollection<EmpresaSaldoMovimento> EmpresaSaldoMovimentos { get; set; }
		public ICollection<TransferenciaQuarto> TransferenciaQuartos { get; set; }
		


    }


}