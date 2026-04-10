using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Categoria : BaseDomainEntity
    {
        public string Nome { get; set; }
        public string CaminhoImagem { get; set; }
        public ICollection<Produtos> Produtos { get; set; }
    }
}