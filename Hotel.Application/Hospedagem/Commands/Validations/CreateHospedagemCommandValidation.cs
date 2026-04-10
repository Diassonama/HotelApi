using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.Hospedagem.Commands.Validations
{
    public class CreateHospedagemCommandValidation: AbstractValidator<CreateHospedagemCommand>
    {
        public CreateHospedagemCommandValidation()
        {
            RuleFor(p=>p.ValorDiaria).NotEmpty();
        }
    }
}