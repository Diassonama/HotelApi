using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Enums;
using Hotel.Domain.Entities;

namespace Hotel.Application.DTOs
{
    public class EmpresaSaldoMovimentoDto
    {
         public int Id { get; set; }
        public int EmpresaSaldoId { get; set; }
        public DateTime Data { get; set; }
        public decimal Valor { get; set; }
        public string NomeEmpresa { get; set; }
        public TipoLancamento TipoLancamento { get; set; }
        public  Domain.Entities.EmpresaSaldo EmpresaSaldo { get; set; } 
        public Utilizador Utilizador { get; set; }
        public string Documento { get; set; }
        public string Observacao { get; set; }
        public string UtilizadorId { get; set; }
        public string NomeUtilizador { get; set; }
    }
}