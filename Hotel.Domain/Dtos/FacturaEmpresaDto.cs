using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Dtos
{
    public class FacturaEmpresaDto
    {
        public int EmpresasId { get; set; }
         public string NomeEmpresa { get; set; }
          public int QuantidadeDeFaturas { get; set; }
           public float ValorTotalDivida { get; set; }

    }
}