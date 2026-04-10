using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Hotel.Domain.Entities
{
    public class TaxAccountingBasis
    {
        [Key]
        public string TaxAccounting { get; set; }
        public string AccountingDescription { get; set; }
    }
    
}
