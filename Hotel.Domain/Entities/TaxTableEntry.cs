using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class TaxTableEntry
    {
        public int Id { get; set; }
        public string TaxCountryRegion { get; set; }
        public string TaxCode { get; set; }
        public string Description { get; set; }
        public string TaxType { get; set; }
        public DateTime TaxExpirationDate { get; set; }
        public float TaxPercentage { get; set; }
        public float TaxAmount { get; set; }
        public TaxTypes TaxTypes { get; set; }
     //   public Tax Tax { get; set; }

        public ICollection<TaxExemptionReason> TaxExemptionReasons { get; set; }
        public ICollection<Produtos> Produtos { get; set; }
    }
}