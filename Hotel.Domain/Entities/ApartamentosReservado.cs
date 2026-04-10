using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class ApartamentosReservado : BaseDomainEntity
    {

        public int ReservasId { get; private set; }
        public int ApartamentosId { get; private set; }
        public DateTime DataEntrada { get; private set; }
        public DateTime DataSaida { get; private set; }
        public int ClientesId { get; private set; }
        public int TipoHospedagensId { get; private set; }
        public string? UtilizadoresId { get; private set; }
        public decimal ValorDiaria { get; private set; }
        public decimal Total { get; private set; }
        public int QuantidadeDeDias { get; private set; }
        public bool ReservaConfirmada { get; private set; }
        public bool ReservaNoShow { get; private set; }

        // Navegação
        public Reserva Reservas { get; private set; }
        public Apartamentos Apartamentos { get; private set; }
        public Cliente Clientes { get; private set; }
        public TipoHospedagem TipoHospedagens { get; private set; }
        public Utilizador Utilizadores { get; private set; }

        public ApartamentosReservado()
        {
            
        }

        // Construtor para garantir consistência na criação da entidade
        public ApartamentosReservado(int reservaId, int apartamentoId, DateTime dataEntrada, DateTime dataSaida, 
            int clienteId, int tipoHospedagemId, string? utilizadorId, decimal valorDiaria, bool reservaConfirmada, bool reservaNoShow)
        {
            ReservasId = reservaId;
            ApartamentosId = apartamentoId;
            DataEntrada = dataEntrada;
            DataSaida = dataSaida;
            ClientesId = clienteId;
            TipoHospedagensId = tipoHospedagemId;
            UtilizadoresId = utilizadorId;
            ValorDiaria = valorDiaria;
            ReservaConfirmada = reservaConfirmada;
            ReservaNoShow = reservaNoShow;
            CalcularTotal();
        }

        // Método para calcular o total automaticamente
        private void CalcularTotal()
        {
            QuantidadeDeDias = (int)(DataSaida - DataEntrada).TotalDays;
            QuantidadeDeDias = QuantidadeDeDias > 0 ? QuantidadeDeDias : 1;
            Total = ValorDiaria * QuantidadeDeDias;
        }

        // Método para atualizar dados do apartamento reservado
        public void AtualizarDados(DateTime dataEntrada, DateTime dataSaida, decimal valorDiaria, bool reservaConfirmada, bool reservaNoShow)
        {
            DataEntrada = dataEntrada;
            DataSaida = dataSaida;
            ValorDiaria = valorDiaria;
            ReservaConfirmada = reservaConfirmada;
            ReservaNoShow = reservaNoShow;
            CalcularTotal();
        }
    }
}