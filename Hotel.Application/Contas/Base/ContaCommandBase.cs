using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.Contas.Base
{
    public class ContaCommandBase: IRequest<BaseCommandResponse>
    {
        public decimal Valor { get; set; }
        public DateTime? Vencimento { get; set; }
        public string Documento { get; set; }
        public string Observacao { get; set; }
    }
}