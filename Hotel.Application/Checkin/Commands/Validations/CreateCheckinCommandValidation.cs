using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.Checkin.Commands.Validations
{
    public class CreateCheckinCommandValidation: AbstractValidator<CreateCheckinCommand>
    {
        public CreateCheckinCommandValidation()
        {
            RuleFor(m=>m.DataEntrada).NotEmpty();
        }
    }
}