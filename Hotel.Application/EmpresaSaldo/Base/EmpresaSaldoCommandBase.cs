using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.EmpresaSaldo.Base
{
    public class EmpresaSaldoCommandBase: IRequest<BaseCommandResponse>
    {
        [Required(ErrorMessage = "ID da empresa é obrigatório")]
        public int EmpresaId { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Documento é obrigatório")]
        [StringLength(50)]
        public string Documento { get; set; }

       /*  [Required(ErrorMessage = "Utilizador é obrigatório")]
        public string UtilizadorId { get; set; } */

        [StringLength(500)]
        public string Observacao { get; set; }
        
    }
}