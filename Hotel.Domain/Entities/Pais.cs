using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
     public class Pais: BaseDomainEntity
    {
        public string Nome { get; set; }
        public string Nacionalidade { get; set; }
        public ICollection<Cliente> Clientes { get; set; }
    }
}