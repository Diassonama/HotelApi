using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.TipoApartamento.Commands.Validations
{
    public class CreateTipoApartamentoCommandValidation : AbstractValidator<CreateTipoApartamentoCommand>
    {
        public CreateTipoApartamentoCommandValidation()
        {
            RuleFor(m => m.Descricao).NotEmpty();
            RuleFor(m => m.ValorDiariaSingle).GreaterThan(0);
            RuleFor(m => m.ValorDiariaDouble).GreaterThan(0);
            RuleFor(m => m.ValorDiariaTriple).GreaterThan(0);
            RuleFor(m => m.ValorDiariaQuadruple).GreaterThan(0);
            RuleFor(m => m.ValorUmaHora).GreaterThan(0);
            RuleFor(m => m.ValorDuasHora).GreaterThan(0);
            RuleFor(m => m.ValorTresHora).GreaterThan(0);
            RuleFor(m => m.ValorQuatroHora).GreaterThan(0);
            RuleFor(m => m.ValorNoite).GreaterThan(0);
            RuleFor(m => m.Segunda).GreaterThan(0);
            RuleFor(m => m.Terca).GreaterThan(0);
            RuleFor(m => m.Quarta).GreaterThan(0);
            RuleFor(m => m.Quinta).GreaterThan(0);
            RuleFor(m => m.Sexta).GreaterThan(0);
            RuleFor(m => m.Sabado).GreaterThan(0);
            RuleFor(m => m.Domingo).GreaterThan(0);
            
        }
    }
}