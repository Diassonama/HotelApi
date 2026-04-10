using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class MobiliaTipoApartamento: BaseDomainEntity
    {
        public ICollection<Patrimonio> patrimonios  { get; set; }
        public ICollection<TipoApartamento> TipoApartamentos { get; set; }
    }
}