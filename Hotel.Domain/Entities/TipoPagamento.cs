using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class TipoPagamento:BaseDomainEntity
    {
        public string Descricao { get; set; }
      //  public ICollection<Pagamento> Pagamentos { get; set; }
      //  public ICollection<Reserva> Reservas { get; set; }
        public ICollection<LancamentoCaixa> LancamentoCaixas { get; set; }

    }
}