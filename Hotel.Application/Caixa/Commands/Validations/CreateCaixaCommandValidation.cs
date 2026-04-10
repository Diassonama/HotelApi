using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.Caixa.Commands.Validations
{
    public class CreateCaixaCommandValidation: AbstractValidator<CreateCaixaCommand>
    {
        public CreateCaixaCommandValidation()
        {
            RuleFor(m=> m.SaldoInicial).NotEmpty();
        }
    }
}