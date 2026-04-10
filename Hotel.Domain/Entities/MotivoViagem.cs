using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
   public class MotivoViagem: BaseDomainEntity
    {
        public string Descricao { get; set; }
        public ICollection<Hospedagem> Hospedagens { get; set; }
    }
}