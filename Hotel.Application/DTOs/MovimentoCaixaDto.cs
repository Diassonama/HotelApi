using System;
using System.Collections.Generic;

namespace Hotel.Application.Dtos
{
    public class MovimentoCaixaDto
    {
        public string NomeHotel { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string NumContribuinte { get; set; }
        public string Telefone { get; set; }
        public string LogoCaminho { get; set; }
        public DateTime DataRelatorio { get; set; }
        public DateTime DataImpressao { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public string Periodo { get; set; }
        public string UsuarioFiltrado { get; set; }
        public List<MovimentoCaixaLinhaDto> Linhas { get; set; } = new();
        public float TotalEntradas { get; set; }
        public float TotalSaidas { get; set; }
    }

    public class MovimentoCaixaLinhaDto
    {
        public string Data { get; set; }
        public string FormaPagamento { get; set; }
        public string Observacao { get; set; }
        public string Operador { get; set; }
        public float Entradas { get; set; }
        public float Saidas { get; set; }
    }
}
