using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class Tax
    {
        public int Id { get; set; }
        public string TaxCode { get; set; }
        public string Descricao { get; set; }
        public string TaxType { get; set; }
        public TaxTypes TaxTypes { get; set; }
      //  public ICollection<TaxTableEntry> TaxTableEntry { get; set; }
    }
}