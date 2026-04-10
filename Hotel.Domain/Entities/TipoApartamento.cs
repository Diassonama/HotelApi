using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;
using Hotel.Domain.Enums;

namespace Hotel.Domain.Entities
{
    public class TipoApartamento : BaseDomainEntity
    {
        public string Descricao { get; private set; }
        public float ValorDiariaSingle { get; private set; }
        public float ValorDiariaDouble { get; private set; }
        public float ValorDiariaTriple { get; private set; }
        public float ValorDiariaQuadruple { get; private set; }
        public float ValorUmaHora { get; private set; }
        public float ValorDuasHora { get; private set; }
        public float ValorTresHora { get; private set; }
        public float ValorQuatroHora { get; private set; }
        public float ValorNoite { get; private set; }
        public float Segunda { get; private set; }
        public float Terca { get; private set; }
        public float Quarta { get; private set; }
        public float Quinta { get; private set; }
        public float Sexta { get; private set; }
        public float Sabado { get; private set; }
        public float Domingo { get; private set; }
        //   public IDictionary<DayOfWeek, float> ValoresPorDia { get; private set; }
        public ICollection<Apartamentos> Apartamentos { get; set; }
        //  public ICollection<Governanca> Governancas { get; set; }
        //  public ICollection<MobiliaApartamento> MobiliaApartamentos { get; set; }
        // public ICollection<Reserva> Reservas { get; set; }
        public TipoApartamento()
        {

        }


        public TipoApartamento(string descricao, float valorDiariaSingle, float valorDiariaDouble, float valorDiariaTriple,
                                   float valorDiariaQuadruple, float valorUmaHora, float valorDuasHora, float valorTresHora,
                                   float valorQuatroHora, float valorNoite, float segunda, float terca, float quarta, float quinta,
                                   float sexta, float sabado, float domingo
                                   //   IDictionary<DayOfWeek, float> valoresPorDia
                                   ) //: this()
        {
            //  Id = id;
            Descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
            SetValoresDiarias(valorDiariaSingle, valorDiariaDouble, valorDiariaTriple, valorDiariaQuadruple);
            SetValoresHorarios(valorUmaHora, valorDuasHora, valorTresHora, valorQuatroHora, valorNoite);
            SetValoresSemanal(segunda, terca, quarta, quinta, sexta, sabado, domingo);
            //  ValoresPorDia = valoresPorDia ?? throw new ArgumentNullException(nameof(valoresPorDia));
            DateCreated = DateTime.Now;
            IsActive = true;
        }
        public TipoApartamento(int id, string descricao, float valorDiariaSingle, float valorDiariaDouble, float valorDiariaTriple,
                                   float valorDiariaQuadruple, float valorUmaHora, float valorDuasHora, float valorTresHora,
                                   float valorQuatroHora, float valorNoite, float segunda, float terca, float quarta, float quinta, float sexta, float sabado, float domingo
                                   //  IDictionary<DayOfWeek, float> valoresPorDia
                                   )
        {
            if (id <= 0) throw new ArgumentException("Id devem ser positivos.");

            Id = id;
            Descricao = descricao ?? throw new ArgumentNullException(nameof(descricao));
            SetValoresDiarias(valorDiariaSingle, valorDiariaDouble, valorDiariaTriple, valorDiariaQuadruple);
            SetValoresHorarios(valorUmaHora, valorDuasHora, valorTresHora, valorQuatroHora, valorNoite);
            SetValoresSemanal(segunda, terca, quarta, quinta, sexta, sabado, domingo);
                   //  ValoresPorDia = valoresPorDia ?? throw new ArgumentNullException(nameof(valoresPorDia));
        }
        // Métodos para definir os valores de diárias com validações
        public void SetValoresDiarias(float single, float doubleRate, float triple, float quadruple)
        {
            if (single <= 0 || doubleRate <= 0 || triple <= 0 || quadruple <= 0)
                throw new ArgumentException("Valores de diária devem ser positivos.");

            ValorDiariaSingle = single;
            ValorDiariaDouble = doubleRate;
            ValorDiariaTriple = triple;
            ValorDiariaQuadruple = quadruple;
        }

        // Métodos para definir valores por hora com validações
        public void SetValoresHorarios(float umaHora, float duasHora, float tresHora, float quatroHora, float noite)
        {
            if (umaHora <= 0 || duasHora <= 0 || tresHora <= 0 || quatroHora <= 0 || noite <= 0)
                throw new ArgumentException("Valores de hora devem ser positivos.");

            ValorUmaHora = umaHora;
            ValorDuasHora = duasHora;
            ValorTresHora = tresHora;
            ValorQuatroHora = quatroHora;
            ValorNoite = noite;
        }

        public void SetValoresSemanal(float segunda, float terca, float quarta, float quinta, float sexta, float sabado, float domingo)
        {
            if (segunda <= 0 || terca <= 0 || quarta <= 0 || quinta <= 0 || sexta <= 0 || sabado <= 0 || domingo <= 0)
                throw new ArgumentException("Valores de semanal devem ser positivos.");

            Segunda = segunda;
            Terca = terca;
            Quarta = quarta;
            Quinta = quinta;
            Sexta = sexta;
            Sabado = sabado;
            Domingo = domingo;
    }

    // Métodos adicionais, por exemplo, para calcular tarifas com base em dia da semana e tipo de acomodação
        /*  public float CalcularTarifa(DayOfWeek dia, int ocupacao)
         {
             if (!ValoresPorDia.TryGetValue(dia, out float valorBase))
                 throw new ArgumentException("Dia da semana inválido.");

             return ocupacao switch
             {
                 1 => valorBase + ValorDiariaSingle,
                 2 => valorBase + ValorDiariaDouble,
                 3 => valorBase + ValorDiariaTriple,
                 4 => valorBase + ValorDiariaQuadruple,
                 _ => throw new ArgumentException("Ocupação inválida.")
             };

         } */

    }
}