using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Despertador:BaseDomainEntity
    {
      //  public int IdCheckin { get; set; }
        public string NomeHospede { get; set; }
        public DateTime DataHoraDespertar { get; set; }

      //  [ForeignKey("IdCheckin")]

        public Checkins Checkins { get; set; }

     
    }
}