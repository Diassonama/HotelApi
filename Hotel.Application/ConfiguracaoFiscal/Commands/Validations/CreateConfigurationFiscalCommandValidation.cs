using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace Hotel.Application.ConfiguracaoFiscal.Commands.Validations
{
    public class CreateConfigurationFiscalCommandValidation : AbstractValidator<CreateConfiguracaoFiscalCommand>
    {
        public CreateConfigurationFiscalCommandValidation()
        {
            RuleFor(m=>m.IVA).NotEmpty();
            RuleFor(m=>m.Estabelecimento).NotEmpty();
        }
    }
}