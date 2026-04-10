using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class TipoRecibo: BaseDomainEntity
    {
        public string Descricao  {get; set;} 
        public string RPT {get; set;} 
        
    }
}