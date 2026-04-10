using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Dtos
{
    public class ReservaDto
    {
         public int ReservaId { get; set; }
        public DateTime DataEntrada { get; set; }
        public DateTime DataSaida { get; set; }
        public int NPX { get; set; }
        public float PagamentoAntecipado { get; set; }
        public string Observacao { get; set; }
        public string Grupo { get; set; }
        public float Total { get; set; }
        public ClienteDto Cliente { get; set; }
        public EmpresaDto Empresa { get; set; }
        public TipoHospedagemDto TipoHospedagem { get; set; }
        public List<ReservaApartamentoDto> ApartamentosReservados { get; set; }
    }
}