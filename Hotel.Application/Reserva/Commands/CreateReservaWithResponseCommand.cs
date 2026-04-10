using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Reserva.Base;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Dtos;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Reserva.Commands
{
    /// <summary>
    /// Comando para criar reserva que retorna resposta estruturada com mensagens de erro detalhadas
    /// </summary>
    public class CreateReservaWithResponseCommand : ReservaCommandBase, IRequest<BaseCommandResponse>
    {
        public class CreateReservaWithResponseCommandHandler : IRequestHandler<CreateReservaWithResponseCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly UsuarioLogado _usuario;

            public CreateReservaWithResponseCommandHandler(IUnitOfWork unitOfWork, UsuarioLogado usuario)
            {
                _unitOfWork = unitOfWork;
                _usuario = usuario;
            }

            public async Task<BaseCommandResponse> Handle(CreateReservaWithResponseCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();

                try
                {
                    // ✅ VERIFICAÇÃO DE DISPONIBILIDADE: Validar se todos os apartamentos estão disponíveis ANTES de criar a reserva
                    foreach (var apto in request.ApartamentosReservados)
                    {
                        var (isDisponivel, mensagemErro) = await _unitOfWork.Reservas.VerificarDisponibilidadeAsync(
                            apto.ApartamentosId,
                            apto.DataEntrada,
                            apto.DataSaida);

                        if (!isDisponivel)
                        {
                            // ✅ ERRO DETALHADO PARA O USUÁRIO: Retorna mensagem clara e específica
                            response.Success = false;
                            response.Message = $"❌ Não foi possível criar a reserva: {mensagemErro}";
                            response.Errors = new List<string> { 
                                $"🏨 Apartamento: {apto.ApartamentosId}",
                                $"📅 Período solicitado: {apto.DataEntrada:dd/MM/yyyy} a {apto.DataSaida:dd/MM/yyyy}",
                                $"⚠️ Motivo: {mensagemErro}"
                            };
                            return response;
                        }
                    }

                    // Primeiro, criar e salvar a reserva para obter o ID
                    var reserva = new Domain.Entities.Reserva(request.EmpresaId, request.QuantidadeQuartos, request.NPX);
                    await _unitOfWork.Reservas.Add(reserva);
                    await _unitOfWork.Save(); // ✅ Salvar primeiro para obter o ID da reserva

                    // Agora adicionar os apartamentos usando o ID da reserva criada
                    foreach (var apto in request.ApartamentosReservados)
                    {
                        var apartamentoReservado = new ApartamentosReservado
                        (
                            reserva.Id, // ✅ Agora o ID da reserva está disponível
                            apto.ApartamentosId,
                            apto.DataEntrada,
                            apto.DataSaida,
                            apto.ClientesId,
                            apto.TipoHospedagensId,
                            _usuario.IdUtilizador,
                            apto.ValorDiaria,
                            apto.ReservaConfirmada,
                            apto.ReservaNoShow
                        );

                        // Adicionar na entidade de domínio
                        reserva.AdicionarApartamentoReservado(apartamentoReservado);
                        
                        // Adicionar no contexto para persistir
                        await _unitOfWork.apartamentoReservado.Add(apartamentoReservado);
                    }

                    // Salvar todas as mudanças (apartamentos reservados)
                    await _unitOfWork.Save();

                    // ✅ Sucesso - retorna resposta estruturada com detalhes para o usuário
                    response.Success = true;
                    response.Message = "✅ Reserva criada com sucesso!";
                    response.Data = new { 
                        reservaId = reserva.Id,
                        quantidadeApartamentos = request.ApartamentosReservados.Count,
                        valorTotal = request.ApartamentosReservados.Sum(a => a.ValorDiaria),
                        apartamentos = request.ApartamentosReservados.Select(a => new {
                            apartamentoId = a.ApartamentosId,
                            dataEntrada = a.DataEntrada.ToString("dd/MM/yyyy"),
                            dataSaida = a.DataSaida.ToString("dd/MM/yyyy"),
                            valorDiaria = a.ValorDiaria
                        }).ToList()
                    };

                    return response;
                }
                catch (Exception ex)
                {
                    // ✅ ERRO DETALHADO PARA O USUÁRIO: Apresenta informações claras sobre falhas do sistema
                    response.Success = false;
                    response.Message = "❌ Erro interno do sistema ao processar a reserva";
                    response.Errors = new List<string> { 
                        "🔧 Detalhes técnicos para suporte:",
                        ex.Message,
                        ex.InnerException?.Message ?? "Nenhum detalhe adicional disponível"
                    };
                    return response;
                }
            }
        }
    }
}
