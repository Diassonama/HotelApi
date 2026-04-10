using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;   
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using MediatR;

namespace Hotel.Application.PlanoDeConta.Base
{
    public class PlanoDeContaCommandBase : IRequest<BaseCommandResponse>
    {
        public string Descricao { get; set; }
        public int ContasId { get; set; }
    }
}
