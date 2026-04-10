using System;

namespace Hotel.Application.DTOs
{
    public class GovernancaArrumacaoDto
    {
        public int CheckinId { get; set; }
        public string Codigo { get; set; }
        public string Hospede { get; set; }
        public string Tipo { get; set; }
        public int Pax { get; set; }
        public DateTime Checkin { get; set; }
        public DateTime Checkout { get; set; }
        public string PaxGov { get; set; }
        public string Limpo { get; set; }
        public string PV { get; set; }
        public string NQA { get; set; }
        public string DF { get; set; }
        public string SB { get; set; }
        public string MB { get; set; }
        public string PB { get; set; }
        public string Observacao { get; set; }
    }
}