using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Reserva.Commands
{
    public class CancelApartamentosReservadosCommand : IRequest<BaseCommandResponse>
    {
        public List<int> ApartamentosReservadosIds { get; set; } = new();
    }

    public class CancelApartamentosReservadosCommandValidator : AbstractValidator<CancelApartamentosReservadosCommand>
    {
        public CancelApartamentosReservadosCommandValidator()
        {
            RuleFor(x => x.ApartamentosReservadosIds)
                .NotEmpty()
                .WithMessage("Deve haver pelo menos um apartamento reservado para cancelar.");

            RuleForEach(x => x.ApartamentosReservadosIds)
                .GreaterThan(0)
                .WithMessage("IDs dos apartamentos reservados devem ser maiores que zero.");
        }
    }

    public class CancelApartamentosReservadosCommandHandler : IRequestHandler<CancelApartamentosReservadosCommand, BaseCommandResponse>
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<CancelApartamentosReservadosCommand> _validator;

        public CancelApartamentosReservadosCommandHandler(
            IReservaRepository reservaRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IValidator<CancelApartamentosReservadosCommand> validator)
        {
            _reservaRepository = reservaRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<BaseCommandResponse> Handle(CancelApartamentosReservadosCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();

            try
            {
                // Validação
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    response.Success = false;
                    response.Message = "Dados inválidos";
                    response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return response;
                }

                // Cancelar apartamentos reservados
                var sucesso = await _reservaRepository.CancelarApartamentosReservadosAsync(request.ApartamentosReservadosIds);

                if (sucesso)
                {
                    response.Success = true;
                    response.Message = "Apartamentos reservados cancelados com sucesso";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Nenhum apartamento reservado foi encontrado para cancelar";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Erro interno do servidor";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
