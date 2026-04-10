using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Hotel.Application.Responses;
using Hotel.Domain.Dtos;
using MediatR;

namespace Hotel.Application.Pagamento.Base
{
    public class PagamentoCommandBase :IRequest<BaseCommandResponse>
    {
    public PagamentoRequest pagamentoRequest  { get; set; }
    }
}