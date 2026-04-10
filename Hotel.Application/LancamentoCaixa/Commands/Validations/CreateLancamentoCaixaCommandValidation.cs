using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.LancamentoCaixa.Commands.Validations
{
    public class CreateLancamentoCaixaCommandValidation: AbstractValidator<CreateLancamentoCaixaCommand>
    {
        public CreateLancamentoCaixaCommandValidation()
        {
            RuleFor(m=>m.CaixasId).GreaterThan(0).NotEmpty();
            RuleFor(m=>m.TipoPagamentosId).GreaterThan(0);
            RuleFor(m=>m.Valor).GreaterThan(0).NotEmpty();
            RuleFor(m=>m.ValorPago).GreaterThan(0).NotEmpty();
        }
    }
}