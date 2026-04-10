using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.Caixa.Commands
{
    public class AnularCaixaCommand: IRequest<BaseCommandResponse>
    {
        public int checkinId { get; set; }
        public class AnularCaixaCommandHandler : IRequestHandler<AnularCaixaCommand, BaseCommandResponse>
        {
            public Task<BaseCommandResponse> Handle(AnularCaixaCommand request, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}