using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.MenuRole.Commands.Validations
{
    public class CreateMenuRoleValidation : AbstractValidator<CreateMenuRoleCommand>
    {
        public CreateMenuRoleValidation()
        {
            RuleFor(m=> m.command[0].MenuId).NotEmpty();
            RuleFor(m=> m.command[0].RoleId).NotEmpty();

        }
    }
}