using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Dtos;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Reserva.Commands
{
    public class AddApartamentosToReservaCommand : IRequest<BaseCommandResponse>
    {
        public int ReservaId { get; set; }
        public List<ReservaApartamentoDto> Apartamentos { get; set; } = new();
    }

    public class AddApartamentosToReservaCommandValidator : AbstractValidator<AddApartamentosToReservaCommand>
    {
        public AddApartamentosToReservaCommandValidator()
        {
            RuleFor(x => x.ReservaId)
                .GreaterThan(0)
                .WithMessage("ReservaId deve ser maior que zero.");

            RuleFor(x => x.Apartamentos)
                .NotEmpty()
                .WithMessage("Deve haver pelo menos um apartamento para adicionar.");

            RuleForEach(x => x.Apartamentos).SetValidator(new ReservaApartamentoDtoValidator());
        }
    }

    public class ReservaApartamentoDtoValidator : AbstractValidator<ReservaApartamentoDto>
    {
        public ReservaApartamentoDtoValidator()
        {
            RuleFor(x => x.ApartamentosId)
                .GreaterThan(0)
                .WithMessage("ApartamentosId deve ser maior que zero.");

            RuleFor(x => x.DataEntrada)
                .NotEmpty()
                .WithMessage("Data de entrada é obrigatória.")
                .GreaterThanOrEqualTo(DateTime.Today)
                .WithMessage("Data de entrada não pode ser anterior a hoje.");

            RuleFor(x => x.DataSaida)
                .NotEmpty()
                .WithMessage("Data de saída é obrigatória.")
                .GreaterThan(x => x.DataEntrada)
                .WithMessage("Data de saída deve ser posterior à data de entrada.");

            RuleFor(x => x.ClientesId)
                .GreaterThan(0)
                .WithMessage("ClientesId deve ser maior que zero.");

            RuleFor(x => x.TipoHospedagensId)
                .GreaterThan(0)
                .WithMessage("TipoHospedagensId deve ser maior que zero.");

           /*  RuleFor(x => x.UtilizadoresId)
                .NotEmpty()
                .WithMessage("UtilizadoresId é obrigatório."); */

            RuleFor(x => x.ValorDiaria)
                .GreaterThan(0)
                .WithMessage("Valor da diária deve ser maior que zero.");
        }
    }

    public class AddApartamentosToReservaCommandHandler : IRequestHandler<AddApartamentosToReservaCommand, BaseCommandResponse>
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UsuarioLogado _usuario;
        private readonly IValidator<AddApartamentosToReservaCommand> _validator;

        public AddApartamentosToReservaCommandHandler(
            IReservaRepository reservaRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IValidator<AddApartamentosToReservaCommand> validator,
            UsuarioLogado usuario)
        {
            _reservaRepository = reservaRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _usuario = usuario;
        }

        public async Task<BaseCommandResponse> Handle(AddApartamentosToReservaCommand request, CancellationToken cancellationToken)
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

                // ✅ VERIFICAÇÃO DE DISPONIBILIDADE: Validar se todos os apartamentos estão disponíveis ANTES de criar os objetos
                foreach (var apartamento in request.Apartamentos)
                {
                    var (isDisponivel, mensagemErro) = await _reservaRepository.VerificarDisponibilidadeAsync(
                        apartamento.ApartamentosId,
                        apartamento.DataEntrada,
                        apartamento.DataSaida);

                    if (!isDisponivel)
                    {
                        response.Success = false;
                        response.Message = "Apartamento não disponível";
                        response.Errors = new List<string> { mensagemErro };
                        return response;
                    }
                }

                // Criar apartamentos reservados (só executa se todos estiverem disponíveis)
                var apartamentosReservados = request.Apartamentos.Select(ar => 
                    new ApartamentosReservado(
                        request.ReservaId,
                        ar.ApartamentosId,
                        ar.DataEntrada,
                        ar.DataSaida,
                        ar.ClientesId,
                        ar.TipoHospedagensId,
                        _usuario.UserId,
                        ar.ValorDiaria,
                        ar.ReservaConfirmada,
                        ar.ReservaNoShow)).ToList();

                // Inserir apartamentos na reserva
                var sucesso = await _reservaRepository.InserirApartamentosReservadosAsync(request.ReservaId, apartamentosReservados);

                if (sucesso)
                {
                    response.Success = true;
                    response.Message = "Apartamentos adicionados à reserva com sucesso";
                    response.Data = new { reservaId = request.ReservaId };
                }
                else
                {
                    response.Success = false;
                    response.Message = "Falha ao adicionar apartamentos à reserva";
                }
            }
            catch (ArgumentException ex)
            {
                response.Success = false;
                response.Message = "Reserva não encontrada";
                response.Errors = new List<string> { ex.Message };
            }
            catch (InvalidOperationException ex)
            {
                response.Success = false;
                response.Message = "Erro de validação";
                response.Errors = new List<string> { ex.Message };
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
