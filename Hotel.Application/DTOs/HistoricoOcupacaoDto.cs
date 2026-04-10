using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.DTOs
{
    public class HistoricoOcupacaoDto
    {
        public int Checkin { get; set; }               // ex: número do checkin
        public string Quarto { get; set; }             // ex: código do apartamento
        public string Hospede { get; set; }            // nome do hóspede
        public string Empresa { get; set; }            // nome da empresa/conta
        public DateTime DataAbertura { get; set; }     // data de abertura
        public string Checkout { get; set; }           // texto do checkout (Hoje / Amanhã / Em X dia(s) / Check out Atrasado)
    }
}