using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
   public class MenuAcesso
    {
        public int IdMenu { get; set; }
        public string Nome { get; set; }
        public bool check { get; set; }
    }
}