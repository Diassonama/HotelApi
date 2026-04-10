using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.Roles.Commands.Validations
{
    public class CreateRoleCommandValidation : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidation()
        {
            RuleFor(m=> m.Nome).NotEmpty().WithMessage("Nome é obrigatório");
        }
    }
}