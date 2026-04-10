using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.DTOs
{
    public class EmpresaSaldoDto
    {
         public int Id { get; set; }
        public int EmpresaId { get; set; }
        public string NomeEmpresa { get; set; }
        public decimal Saldo { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public List<EmpresaSaldoMovimentoDto> Movimentacoes { get; set; } = new List<EmpresaSaldoMovimentoDto>();

    }
}