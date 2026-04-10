using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.Apartamentos.Commands.Validations;
using Hotel.Application.Checkin.Commands;
using Hotel.Application.Helper;
using Hotel.Application.Hospedagem.Base;
using Hotel.Application.Hospedes.Commands;
using Hotel.Application.Interfaces;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Hospedagem.Commands
{
    public class CreateHospedagemCommand : HospedagemCommandBase
    {
        public class CreateHospedagemCommandHandler : IRequestHandler<CreateHospedagemCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IValidator<CreateHospedagemCommand> _validator;
            private readonly IMediator _mediator;
            private readonly UsuarioLogado _usuarioLogado;
            private readonly ICaixa _caixa;
            private readonly IRackNotificationService _rackNotificationService;

            public CreateHospedagemCommandHandler(
                IUnitOfWork unitOfWork,
                IValidator<CreateHospedagemCommand> validator,
                IMediator mediator,
                UsuarioLogado logado,
                ICaixa caixa,
                IRackNotificationService rackNotificationService)
            {
                _unitOfWork = unitOfWork;
                _validator = validator;
                _mediator = mediator;
                _usuarioLogado = logado;
                _caixa = caixa;
                _rackNotificationService = rackNotificationService;
            }
            public async Task<BaseCommandResponse> Handle(CreateHospedagemCommand request, CancellationToken cancellationToken)
            {
               /*  */
                var response = new BaseCommandResponse();

                // Validação inicial
                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    return GenerateErrorResponse("Erros encontrados ao cadastrar hospedagem", validationResult.Errors.Select(e => e.ErrorMessage).ToList());
                }

                try
                {
                    // Validar caixa
                    var caixaId = _caixa.getCaixa;
                    if (caixaId <= 0)
                    {
                        return GenerateErrorResponse("Caixa encontra-se fechado");
                    }

                    // Validar usuário logado
                    if (_usuarioLogado.IdUtilizador == null)
                    {
                        return GenerateErrorResponse("Utilizador não encontrado");
                    }
                   // var angolaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
                    var dataInicialAngola = TimeZoneHelper.GetDateInAngola(request.DataAbertura);
                    var dataFinalAngola   = TimeZoneHelper.GetDateInAngola(request.PrevisaoFechamento);


                    // Calcular valores da hospedagem
                    var totalDias = (dataFinalAngola - dataInicialAngola).Days;
                 //   var totalDias = (request.PrevisaoFechamento.Date - request.DataAbertura.Date).Days;
                    var totalDiaria = request.ValorDiaria * totalDias;

                    // Criar check-in
                    var checkin = new Checkins(request.DataAbertura, totalDiaria);
                    checkin.UtilizadoECaixaCheckin(caixaId, _usuarioLogado.IdUtilizador);
                    await _unitOfWork.checkins.Add(checkin);

                    // Criar hospedagem
                    var hospedagem = new Domain.Entities.Hospedagem(
                        request.DataAbertura.Date,
                        request.PrevisaoFechamento.Date,
                        request.ValorDiaria,
                        request.ApartamentosId,
                        request.QuantidadeHomens,
                        request.QuantidadeMulheres,
                        request.QuantidadeCrianca,
                        request.TipoHospedagensId,
                        request.EmpresasId,
                        request.MotivoViagensId,
                        checkin.Id
                    );

                    await _unitOfWork.Hospedagem.AddAsync(hospedagem);

                    // Atualizar apartamento
                    _unitOfWork.Apartamento.ocuparApartamento(request.ApartamentosId, checkin.Id);
                    var apartamentoAtualizado = await _unitOfWork.Apartamento.GetByIdAsync(request.ApartamentosId);

                    // Criar hóspede
                    await _mediator.Send(new CreateHospedeCommand
                    {
                        clientesId = request.IdCliente,
                        checkinsId = checkin.Id
                    });

                    // Notificações em tempo real para o rack
                    await _rackNotificationService.NotifyCheckinAsync(checkin);
                    if (apartamentoAtualizado != null)
                    {
                        await _rackNotificationService.NotifyApartmentStatusChangeAsync(apartamentoAtualizado);
                    }
                    await _rackNotificationService.NotifyRackUpdateAsync();

                    response.Data = hospedagem;
                    response.Success = true;
                    response.Message = "Hospedagem cadastrada com sucesso";
                }
                catch (Exception ex)
                {
                    response = GenerateErrorResponse($"Erro ao realizar o check-in: {ex.Message}");
                }

                return response;
            }
             private BaseCommandResponse GenerateErrorResponse(string message, IList<string> errors = null)
            {
                return new BaseCommandResponse
                {
                    Success = false,
                    Message = message,
                    Errors = (List<string>)(errors ?? new List<string>())
                };
            }
        }
    }
}