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
    public class InactivateReservaCommand : IRequest<BaseCommandResponse>
    {
        public int ReservaId { get; set; }
        public string? MotivoInativacao { get; set; }
    }

    public class InactivateReservaCommandValidator : AbstractValidator<InactivateReservaCommand>
    {
        public InactivateReservaCommandValidator()
        {
            RuleFor(x => x.ReservaId)
                .GreaterThan(0)
                .WithMessage("ID da reserva deve ser maior que zero.");

            RuleFor(x => x.MotivoInativacao)
                .MaximumLength(500)
                .WithMessage("Motivo da inativação não pode exceder 500 caracteres.");
        }
    }

    public class InactivateReservaCommandHandler : IRequestHandler<InactivateReservaCommand, BaseCommandResponse>
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<InactivateReservaCommand> _validator;

        public InactivateReservaCommandHandler(
            IReservaRepository reservaRepository,
            IUnitOfWork unitOfWork,
            IValidator<InactivateReservaCommand> validator)
        {
            _reservaRepository = reservaRepository;
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<BaseCommandResponse> Handle(InactivateReservaCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();

            try
            {
                // Validação
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    response.Success = false;
                    response.Message = "❌ Dados inválidos para inativação da reserva";
                    response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return response;
                }

                // Buscar a reserva com apartamentos
                var reserva = await _unitOfWork.Reservas.ObterReservaComApartamentosAsync(request.ReservaId);
                
                if (reserva == null)
                {
                    response.Success = false;
                    response.Message = "❌ Reserva não encontrada";
                    response.Errors = new List<string> { $"Reserva com ID {request.ReservaId} não foi encontrada no sistema" };
                    return response;
                }

                if (!reserva.IsActive)
                {
                    response.Success = false;
                    response.Message = "⚠️ Reserva já está inativa";
                    response.Errors = new List<string> { "A reserva já foi inativada anteriormente" };
                    return response;
                }

                // Verificar se há hospedagens ativas (check-ins realizados)
                var temHospedagensAtivas = await _reservaRepository.VerificarHospedagensAtivasAsync(request.ReservaId);
                
                if (temHospedagensAtivas)
                {
                    response.Success = false;
                    response.Message = "❌ Não é possível inativar reserva com hospedagens ativas";
                    response.Errors = new List<string> 
                    { 
                        "A reserva possui hospedagens ativas (check-ins realizados)",
                        "Finalize as hospedagens antes de inativar a reserva"
                    };
                    return response;
                }

                // Inativar a reserva
                reserva.IsActive = false;
                
                // Adicionar motivo se fornecido
                if (!string.IsNullOrWhiteSpace(request.MotivoInativacao))
                {
                    // Se houver um campo para motivo na entidade, adicione aqui
                    // reserva.MotivoInativacao = request.MotivoInativacao;
                }

                // Inativar todos os apartamentos reservados relacionados
                foreach (var apartamentoReservado in reserva.ApartamentosReservados)
                {
                    apartamentoReservado.IsActive = false;
                }

                // Salvar mudanças
                _unitOfWork.Reservas.Update(reserva);
                await _unitOfWork.Save();

                response.Success = true;
                response.Message = "✅ Reserva inativada com sucesso";
                response.Data = new 
                {
                    ReservaId = reserva.Id,
                    DataInativacao = DateTime.Now,
                    ApartamentosAfetados = reserva.ApartamentosReservados.Count(),
                    MotivoInativacao = request.MotivoInativacao ?? "Não informado"
                };

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "❌ Erro interno do servidor ao inativar reserva";
                response.Errors = new List<string> { ex.Message };
            }

            return response;
        }
    }
}
