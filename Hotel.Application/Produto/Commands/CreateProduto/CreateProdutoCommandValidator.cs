using FluentValidation;

namespace Hotel.Application.Produto.Commands.CreateProduto
{
    public class CreateProdutoCommandValidator : AbstractValidator<CreateProdutoCommand>
    {
        public CreateProdutoCommandValidator()
        {
            RuleFor(p => p.Nome)
                .NotEmpty().WithMessage("{PropertyName} é obrigatório.")
                .NotNull()
                .MaximumLength(100).WithMessage("{PropertyName} deve ter no máximo 100 caracteres.");

            RuleFor(p => p.Valor)
                .GreaterThan(0).WithMessage("{PropertyName} deve ser maior que zero.");

            RuleFor(p => p.Quantidade)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} deve ser maior ou igual a zero.");

            RuleFor(p => p.EstoqueMinino)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} deve ser maior ou igual a zero.");

            RuleFor(p => p.PrecoCompra)
                .GreaterThan(0).WithMessage("{PropertyName} deve ser maior que zero.");

            RuleFor(p => p.MargemLucro)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} deve ser maior ou igual a zero.");

            RuleFor(p => p.CategoriaId)
                .GreaterThan(0).WithMessage("{PropertyName} é obrigatório.");

            RuleFor(p => p.PontoDeVendasId)
                .GreaterThan(0).WithMessage("{PropertyName} é obrigatório.");

            RuleFor(p => p.DataExpiracao)
                .GreaterThan(System.DateTime.Now).WithMessage("{PropertyName} deve ser uma data futura.");
        }
    }
}