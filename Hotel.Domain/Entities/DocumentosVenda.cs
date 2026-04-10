using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class DocumentosVenda
    {
        [Key]
        public string Documento { get; set; }
        public string Descricao { get; set; }
        public string Diario { get; set; }
        public string PagarReceber { get; set; }
        public string TipoConta { get; set; }
        public string Estado { get; set; }
    }
}