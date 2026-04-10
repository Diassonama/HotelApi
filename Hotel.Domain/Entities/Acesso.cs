using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Acesso : BaseDomainEntity
    {
        public string Nome { get; set; }
        public string PreIcon { get; set; }
        public string PostIcon { get; set; }
        public string Path { get; set; }
        public Perfil Perfils { get; set; }

    }
}