using System;
using System.Collections.Generic;

namespace Hotel.Application.Dtos
{
    public class MovimentoDiarioDto
    {
        public string NomeHotel { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string NumContribuinte { get; set; }
        public string Telefone { get; set; }
        public string LogoCaminho { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public DateTime DataRelatorio { get; set; }
        public DateTime DataImpressao { get; set; }

        public List<MovimentoPagamentoDto> Pagamentos { get; set; } = new();
        public List<MovimentoCheckinDto> Checkins { get; set; } = new();
        public List<MovimentoCheckinDto> Checkouts { get; set; } = new();
        public List<MovimentoHistoricoDto> Historicos { get; set; } = new();
    }

    public class MovimentoPagamentoDto
    {
        public string FormaPagamento { get; set; }
        public DateTime Data { get; set; }
        public string Observacao { get; set; }
        public string Operador { get; set; }
        public float Entradas { get; set; }
        public float Saidas { get; set; }
    }

    public class MovimentoCheckinDto
    {
        public int CheckinId { get; set; }
        public string Quarto { get; set; }
        public string Periodo { get; set; }
        public string Hospede { get; set; }
        public string Empresa { get; set; }
        public string Utilizador { get; set; }
    }

    public class MovimentoHistoricoDto
    {
        public int Numero { get; set; }
        public DateTime DataHora { get; set; }
        public string Observacao { get; set; }
        public string Utilizador { get; set; }
    }
}
