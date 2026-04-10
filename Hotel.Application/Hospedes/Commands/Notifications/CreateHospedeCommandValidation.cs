using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.Hospedes.Commands.Notifications
{
    public class CreateHospedeCommandValidation: AbstractValidator<CreateHospedeCommand>
    {
        public CreateHospedeCommandValidation()
        {
            RuleFor(m=>m.clientesId).NotEmpty();
            RuleFor(m=>m.checkinsId).NotEmpty();
        }
    }
}