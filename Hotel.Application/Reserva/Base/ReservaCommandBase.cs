using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Dtos;
using Hotel.Domain.Entities;

namespace Hotel.Application.Reserva.Base
{
    public class ReservaCommandBase
    {
        public int NPX { get; set; }
        public int QuantidadeQuartos { get; set; }
        public int EmpresaId { get; set; }
        public List<ReservaApartamentoDto> ApartamentosReservados { get; set; }
    }
}

