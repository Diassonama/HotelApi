using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class PontoDeVenda : BaseDomainEntity
    {
        public string Nome { get; set; }

        public ICollection<Pedido> Pedidos { get; set; }
        public ICollection<Produtos> Produtos { get; set; }
    }
}