using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.PlanoDeConta.Commands.Validations
{
    public class CreatePlanoDeContaCommandValidation : AbstractValidator<CreatePlanoDeContaCommand>
    {
        public CreatePlanoDeContaCommandValidation()
        {
            RuleFor(p => p.Descricao)
                .NotEmpty().WithMessage("❌ {PropertyName} é obrigatório")
                .NotNull().WithMessage("❌ {PropertyName} é obrigatório")
                .MaximumLength(200).WithMessage("❌ {PropertyName} deve ter no máximo {MaxLength} caracteres");

            RuleFor(p => p.ContasId)
                .NotEmpty().WithMessage("❌ {PropertyName} é obrigatório")
                .GreaterThan(0).WithMessage("❌ {PropertyName} deve ser maior que zero");
        }
    }
}
