using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;

namespace Hotel.Domain.Dtos
{
    public class ApartamentoComCheckinAtivoDto
    {
         public Apartamentos Apartamento { get; set; }
        public Hospedagem Hospedagem { get; set; }
        public Checkins Checkin { get; set; }
        public Hospede Hospede { get; set; }
        public Empresa Empresa { get; set; }
    }
}