using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class TaxTypes
    {
        public string TaxType { get; set; }
		public string Descricao { get; set; }

		public ICollection<Tax> Tax { get; set; }
		public ICollection<TaxTableEntry> TaxTableEntry { get; set; }
    }
}