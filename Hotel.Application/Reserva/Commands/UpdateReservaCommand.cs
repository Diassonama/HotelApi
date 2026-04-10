using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Reserva.Base;
using Hotel.Application.Services;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Reserva.Commands
{
    public class UpdateReservaCommand : ReservaCommandBase, IRequest<bool>
    {
        public int Id { get; set; }

        public class UpdateReservaCommandHandler : IRequestHandler<UpdateReservaCommand, bool>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly UsuarioLogado _usuario;

            public UpdateReservaCommandHandler(IUnitOfWork unitOfWork, UsuarioLogado usuario)
            {
                _unitOfWork = unitOfWork;
                _usuario = usuario;
            }

            public async Task<bool> Handle(UpdateReservaCommand request, CancellationToken cancellationToken)
            {
                var reserva = await _unitOfWork.Reservas.ObterReservaComApartamentosAsync(request.Id);
                if (reserva == null)
                {
                    throw new Exception("Reserva não encontrada.");
                }

                // Atualiza os dados principais
                reserva.AtualizarDados(request.EmpresaId, request.NPX, request.QuantidadeQuartos);

                // 📌 CORREÇÃO: Obtém todos os apartamentos reservados (um ou mais)
                // e faz uma cópia da lista para evitar problemas de modificação durante iteração
                var apartamentosAntigos = reserva.ApartamentosReservados.ToList();
                
                // 📌 Remove TODOS os apartamentos reservados antigos (seja 1 ou vários)
                if (apartamentosAntigos.Any())
                {
                    foreach (var apto in apartamentosAntigos)
                    {
                        // Remove da entidade de domínio
                //        reserva.RemoverApartamentoReservado(apto);
                        
                        // Marca como inativo no banco (soft delete)
                        apto.IsActive = false;
                        await _unitOfWork.apartamentoReservado.Update(apto);
                    }
                }

                // 📌 Adiciona TODOS os novos apartamentos reservados (seja 1 ou vários)
                if (request.ApartamentosReservados != null && request.ApartamentosReservados.Any())
                {
                    foreach (var aptoDto in request.ApartamentosReservados)
                    {
                        // ✅ NOVA FUNCIONALIDADE: Verificar disponibilidade EXCLUINDO a reserva atual
                        var (isDisponivel, mensagemErro) = await _unitOfWork.Reservas.VerificarDisponibilidadeAsync(
                            aptoDto.ApartamentosId, 
                            aptoDto.DataEntrada, 
                            aptoDto.DataSaida,
                            request.Id); // 🎯 Exclui a reserva atual da verificação

                        if (!isDisponivel)
                        {
                            throw new Exception($"Erro na atualização da reserva: {mensagemErro}");
                        }

                        var novoApartamento = new ApartamentosReservado(
                            reserva.Id,
                            aptoDto.ApartamentosId,
                            aptoDto.DataEntrada,
                            aptoDto.DataSaida,
                            aptoDto.ClientesId,
                            aptoDto.TipoHospedagensId,
                            _usuario.IdUtilizador, //aptoDto.UtilizadoresId,
                            aptoDto.ValorDiaria,
                            aptoDto.ReservaConfirmada,
                            aptoDto.ReservaNoShow
                        );

                        // Adiciona na entidade de domínio
                        reserva.AdicionarApartamentoReservado(novoApartamento);
                        
                        // Persiste no banco
                        await _unitOfWork.apartamentoReservado.Add(novoApartamento);
                    }
                }
                else
                {
                    throw new Exception("Uma reserva deve ter pelo menos um apartamento.");
                }

                // Atualiza a reserva no banco
                await _unitOfWork.Reservas.Update(reserva);

                // 📌 Confirma as operações no banco
                await _unitOfWork.Save();

                return true;




                /* 
                            // 📌 Atualiza os dados da reserva
                            reserva = new Domain.Entities.Reserva(
                                request.DataEntrada,
                                request.DataSaida,
                                request.NPX,
                                request.QuantidadeApartamento,
                                request.PagamentoAntecipado,
                                request.Observacao,
                                request.Grupo,
                                0,  // O total será recalculado ao adicionar apartamentos
                                cliente
                            );

                            // 📌 Remove os apartamentos antigos e adiciona os novos
                            reserva.ApartamentosReservados.Clear();
                            int quantidadeDeDias = (int)(request.DataSaida - request.DataEntrada).TotalDays;
                            if (quantidadeDeDias <= 0)
                                throw new ArgumentException("Data de saída deve ser maior que a data de entrada.");

                            foreach (var ap in request.ApartamentosReservados.)
                            {
                                var apartamento = await _apartamentoRepository.GetByIdAsync(ap.ApartamentoId);
                                if (apartamento == null) throw new Exception($"Apartamento ID {ap.ApartamentoId} não encontrado");

                                var apartamentoReservado = new ApartamentosReservado(ap.ApartamentoId, ap.ValorDiaria, quantidadeDeDias, apartamento);
                                reserva.AdicionarApartamento(apartamentoReservado);
                            }

                            await _unitOfWork.Reservas.Update(reserva);
                            return true; */
            }
        }
    }
}