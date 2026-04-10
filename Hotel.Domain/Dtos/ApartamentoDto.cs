using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Dtos
{
    public class ApartamentoDto
    {
         public int Id { get; set; }
        public string Codigo { get; set; }
        public string Tipo { get; set; }
    }
}