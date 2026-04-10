using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class TaxExemptionReason
    {
        [Key]
        public string TaxExemptionCode { get; set; }
        public string TaxExemptionReasons { get; set; }
        public int TaxCode { get; set; }
        public TaxTableEntry TaxTableEntry { get; set; }
        public ICollection<Produtos> Produtos { get; set; }
    }
}