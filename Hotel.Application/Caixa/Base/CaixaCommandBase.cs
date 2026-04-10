using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.Caixa.Base
{
    public class CaixaCommandBase: IRequest<BaseCommandResponse>
    {
         public int Id { get; set; }
         public float SaldoInicial { get; set; }
         public float Valor { get; set; }
    }
}