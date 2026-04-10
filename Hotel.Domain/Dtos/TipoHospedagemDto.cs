using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Dtos
{
    public class TipoHospedagemDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }  // Ex: Standard, Luxo, Suíte Master
        public string Descricao { get; set; }
    }
}