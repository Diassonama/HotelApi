using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.Apartamentos.Commands.Validations
{
    public class CreateApartamentoCommandValidator: AbstractValidator<CreateApartamentoCommand>
    {
       public CreateApartamentoCommandValidator()
       {
        /*  RuleFor(c=> c.Situacao)
            .NotNull().WithMessage("A Situação é obrigatório"); */

         RuleFor(c=> c.Codigo).NotNull().WithMessage("O código é obrigatório");

         RuleFor(c=> c.TipoApartamentosId)
                     .GreaterThan(0) //.WithMessage("O tipo de apartamento não pode ser zero(0)")
                     .NotEmpty().WithMessage("O tipo de apartamento é obrigatório");
         
       /*   RuleFor(c=> c.IsAtivo)
            .NotEmpty().WithMessage("O Estatus é obrigatório");
         RuleFor(c=> c.Frigobar).NotNull().MaximumLength(1); */
            
       }
    }
}