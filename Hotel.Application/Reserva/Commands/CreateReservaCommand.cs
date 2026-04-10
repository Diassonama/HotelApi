using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Reserva.Base;
using Hotel.Application.Services;
using Hotel.Domain.Dtos;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Reserva.Commands
{
    public class CreateReservaCommand :ReservaCommandBase, IRequest<int>
    {

        public class CreateReservaCommandHandler : IRequestHandler<CreateReservaCommand, int>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly UsuarioLogado _usuario;

            public CreateReservaCommandHandler(IUnitOfWork unitOfWork, UsuarioLogado usuario)
            {
                _unitOfWork = unitOfWork;
                _usuario = usuario;
            }

            public async Task<int> Handle(CreateReservaCommand request, CancellationToken cancellationToken)
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
                        // ⚠️ ERRO PARA O USUÁRIO: Mensagem clara sobre a indisponibilidade
                        // NOTA: Este comando retorna apenas int, então a mensagem vai na exceção
                        // Para respostas estruturadas, use CreateReservaWithResponseCommand
                        throw new InvalidOperationException($"❌ Não foi possível criar a reserva: {mensagemErro}");
                    }
                }

                // Primeiro, criar e salvar a reserva para obter o ID
                var reserva = new Domain.Entities.Reserva(request.EmpresaId, request.QuantidadeQuartos, request.NPX);
                await _unitOfWork.Reservas.Add(reserva);
                await _unitOfWork.Save(); // ✅ Salvar primeiro para obter o ID da reserva

                // Agora adicionar os apartamentos usando o ID da reserva criada
                foreach (var apto in request.ApartamentosReservados)
                {
                    /* int reservaId, int apartamentoId, DateTime dataEntrada, DateTime dataSaida, 
                    int clienteId, int tipoHospedagemId, int utilizadorId, float valorDiaria, bool reservaConfirmada, bool reservaNoShow)
          */
                    var apartamentoReservado = new ApartamentosReservado
                    (
                        reserva.Id, // ✅ Agora o ID da reserva está disponível
                        apto.ApartamentosId,
                        apto.DataEntrada,
                        apto.DataSaida,
                        apto.ClientesId,
                        apto.TipoHospedagensId,
                        _usuario.IdUtilizador, //apto.UtilizadoresId,
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
                return reserva.Id;
          
            }
        }
    }
}

