using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Enums;

namespace Hotel.Domain.Entities
{
    public class Transferencia
    {
          public int Id { get; set; }
        public int CheckinId { get; set; }
        public Checkins Checkins { get; set; }
        public int MotivoTransferenciaId { get; set; }

        public int QuartoId { get; set; }
        public Apartamentos Apartamentos { get; set; }

        public TipoTransferencia TipoTransferencia { get; set; }
        public Boolean ManterPreco { get; set; } = true;

        public float ValorDiaria { get; set; }

        public DateTime DataEntrada { get; set; }
        public DateTime DataSaida { get; set; }

        public DateTime DataTransferencia { get; set; }

        public MotivoTransferencia MotivoTransferencia { get; set; }
        public string Observacao { get; set; }
        public string UtilizadorId { get; set; }
        public Utilizador Utilizador { get; set; }
    

    public bool IsSaida => TipoTransferencia == TipoTransferencia.Saida;

        /// <summary>
        /// Indica se este registro representa uma entrada
        /// </summary>
        public bool IsEntrada => TipoTransferencia == TipoTransferencia.Entrada;

        /// <summary>
        /// Descrição do movimento baseada no tipo
        /// </summary>
        public string DescricaoMovimento => TipoTransferencia switch
        {
            TipoTransferencia.Saida => $"Saída do quarto {Apartamentos?.Codigo}",
            TipoTransferencia.Entrada => $"Entrada no quarto {Apartamentos?.Codigo}",
            _ => "Movimento indefinido"
        };

        /// <summary>
        /// Título do movimento para exibição
        /// </summary>
        public string TituloMovimento => TipoTransferencia switch
        {
            TipoTransferencia.Saida => "SAÍDA",
            TipoTransferencia.Entrada => "ENTRADA",
            _ => "INDEFINIDO"
        };
    }
}