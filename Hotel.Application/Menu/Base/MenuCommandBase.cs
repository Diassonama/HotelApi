using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.Menu.Base
{
    public class MenuCommandBase: IRequest<BaseCommandResponse>
    {
        public string PreIcon { get; set; }
        public string PostIcon { get; set; }
        public string Nome { get; set; }
        public string Path { get; set; }
    }
}