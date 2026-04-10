using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
    public class CreateReservaV2Command : IRequest<BaseCommandResponse>
    {
        public int EmpresaId { get; set; }
        public int NPX { get; set; }
        public int QuantidadeQuartos { get; set; }
        public List<ReservaApartamentoDto> Apartamentos { get; set; } = new();
    }

    public class CreateReservaV2CommandValidator : AbstractValidator<CreateReservaV2Command>
    {
        public CreateReservaV2CommandValidator()
        {
            RuleFor(x => x.EmpresaId)
                .GreaterThan(0)
                .WithMessage("EmpresaId deve ser maior que zero.");

            RuleFor(x => x.NPX)
                .GreaterThan(0)
                .WithMessage("NPX deve ser maior que zero.");

            RuleFor(x => x.QuantidadeQuartos)
                .GreaterThan(0)
                .WithMessage("Quantidade de quartos deve ser maior que zero.");

            RuleFor(x => x.Apartamentos)
                .NotEmpty()
                .WithMessage("Deve haver pelo menos um apartamento reservado.");

            RuleForEach(x => x.Apartamentos).SetValidator(new ReservaApartamentoDtoValidator());
        }
    }

    public class CreateReservaV2CommandHandler : IRequestHandler<CreateReservaV2Command, BaseCommandResponse>
    {
        private readonly IReservaRepository _reservaRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UsuarioLogado _usuario;
        private readonly IValidator<CreateReservaV2Command> _validator;

        public CreateReservaV2CommandHandler(
            IReservaRepository reservaRepository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IValidator<CreateReservaV2Command> validator,
            UsuarioLogado usuario)
        {
            _reservaRepository = reservaRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _usuario = usuario;
        }

        public async Task<BaseCommandResponse> Handle(CreateReservaV2Command request, CancellationToken cancellationToken)
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

                // ✅ VERIFICAÇÃO DE DISPONIBILIDADE: Validar se todos os apartamentos estão disponíveis ANTES de criar objetos
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

                // Criar reserva
                var reserva = new Domain.Entities.Reserva(
                    request.EmpresaId,
                    request.NPX,
                    request.QuantidadeQuartos);

                // Criar apartamentos reservados
                var apartamentosReservados = request.Apartamentos.Select(ar => 
                    new ApartamentosReservado(
                        0, // ReservaId será definido após salvar a reserva
                        ar.ApartamentosId,
                        ar.DataEntrada,
                        ar.DataSaida,
                        ar.ClientesId,
                        ar.TipoHospedagensId,
                        _usuario.IdUtilizador, //ar.UtilizadoresId,
                        ar.ValorDiaria,
                        ar.ReservaConfirmada,
                        ar.ReservaNoShow)).ToList();

                // Inserir reserva com apartamentos
                var reservaCriada = await _reservaRepository.InserirReservaComApartamentosAsync(reserva, apartamentosReservados);

                response.Success = true;
                response.Message = "Reserva criada com sucesso";
                response.Data = reservaCriada;
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
