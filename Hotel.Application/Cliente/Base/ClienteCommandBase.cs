using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;   
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Enums;
using MediatR;

namespace Hotel.Application.Cliente.Base
{
    public class ClienteCommandBase :IRequest<BaseCommandResponse>
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public DateTime DataAniversario { get; set; }
        public Genero Generos { get; set; }
        public int EmpresasId { get; set; }
        public int PaisId { get; set; }

    }
}