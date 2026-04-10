using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.Empresa.Commands.Validations
{
    public class CreateEmpresaCommandValidation: AbstractValidator<CreateEmpresaCommand>
    {
        public CreateEmpresaCommandValidation()
        {
            RuleFor(m=>m.RazaoSocial).NotEmpty();
            RuleFor(m=>m.Telefone).NotEmpty();
            RuleFor(m=>m.Email).NotEmpty();
            RuleFor(m=>m.Endereco).NotEmpty();
        }
    }
}