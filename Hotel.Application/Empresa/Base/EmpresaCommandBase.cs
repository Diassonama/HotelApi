using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.Empresa.Base
{
    public class EmpresaCommandBase: IRequest<BaseCommandResponse>
    {
        public string RazaoSocial { get; set; }
        public string Telefone { get; set; }
        public float PercentualDesconto { get; set; }
        public string Email { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string NumContribuinte { get; set; }
    }
}