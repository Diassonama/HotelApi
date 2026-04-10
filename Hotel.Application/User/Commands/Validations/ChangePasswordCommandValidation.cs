using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.User.Commands.changePassword;

namespace Hotel.Application.User.Commands.Validations
{
    public class ChangePasswordCommandValidation: AbstractValidator<ChangePasswordCommad>
    {
        public ChangePasswordCommandValidation()
        {
             RuleFor(v => v.NewPassword)
                .NotEmpty().WithMessage("A Password é obrigatória")
                .Matches("(?=.{10,})((?=.*\\d)(?=.*[a-z])(?=.*[A-Z])|(?=.*\\d)(?=.*[a-zA-Z])(?=.*[\\W_])|(?=.*[a-z])(?=.*[A-Z])(?=.*[\\W_])).*")
                .WithMessage("Tamanho mínimo: 10 caracteres. Utilizar pelo menos três destes: Letra maiúscula, Letra minúscula, Número, Símbolo");

        }
    }
}