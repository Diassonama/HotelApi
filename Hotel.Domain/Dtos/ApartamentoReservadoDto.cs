#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Dtos
{
    public class ReservaApartamentoDto
    {
      
    public int ApartamentosId { get; set; }
    public DateTime DataEntrada { get; set; }
    public DateTime DataSaida { get; set; }
    public int ClientesId { get; set; }
    public int TipoHospedagensId { get; set; }
    //public string? UtilizadoresId { get; set; }
    public decimal ValorDiaria { get; set; }
    public decimal Total { get; set; }
    public bool ReservaConfirmada { get; set; }
    public bool ReservaNoShow { get; set; }
    }
}