using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.Pagamento.Commands.Validations
{
    public class CreatePagamentoCommandValidation: AbstractValidator<CreatePagamentoCommand>
    {
        public CreatePagamentoCommandValidation()
        {
            RuleFor(m=>m.pagamentoRequest.ValorPago).NotEmpty().GreaterThan(0);
        }
    }
}