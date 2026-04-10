using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.Hospedes.Base
{
    public class HospedeCommandBase: IRequest<BaseCommandResponse>
    {
        public int clientesId { get; set; }
        public int checkinsId  { get; set; }
    }
}