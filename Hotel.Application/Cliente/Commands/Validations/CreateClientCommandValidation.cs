using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.Cliente.Commands.Validations
{
    public class CreateClientCommandValidation: AbstractValidator<CreateClienteCommand>
    {
        public CreateClientCommandValidation()
        {
            RuleFor(m=> m.Nome).NotEmpty().WithMessage("Nome é obrigatorio");
            RuleFor(m=> m.EmpresasId).NotEmpty().WithMessage("Empresa é obrigatorio");
            RuleFor(m=> m.PaisId).NotEmpty().WithMessage("Pais é obrigatorio");
        }
    }
}