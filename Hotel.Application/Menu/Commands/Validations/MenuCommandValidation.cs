using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.Menu.Commands.Validations
{
    public class MenuCommandValidation : AbstractValidator<CreateMenuCommand>
    {
        public MenuCommandValidation()
        {
            RuleFor(m=>m.PreIcon).NotEmpty().WithMessage("O PreIcon é obrigatório");
            RuleFor(m=>m.PostIcon).NotEmpty().WithMessage("O PostIcon é obrigatório");
            RuleFor(m=>m.Nome).NotEmpty().WithMessage("O Nome é obrigatório");
            RuleFor(m=>m.Path).NotEmpty().WithMessage("O Path é obrigatório");
        }
    }
}