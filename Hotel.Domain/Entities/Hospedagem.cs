using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;
using static Hotel.Domain.Entities.Hospede;
using static Hotel.Domain.Entities.Pagamento;

namespace Hotel.Domain.Entities
{
    public class Hospedagem : BaseDomainEntity
    {
        // transforme esta classe em dominio rico
        public string Descricao { get; set; }
        public DateTime DataAbertura { get; set; }
        public DateTime? DataFechamento { get; set; } // pode ser null
        public DateTime PrevisaoFechamento { get; set; }
        public Boolean DiariaAntecipada { get; set; } = false;
        public Boolean EarlyCheckin { get; set; } = false;
        public int QuantidadeCrianca { get; private set; }
        public int QuantidadeHomens { get; private set; }
        public int QuantidadeMulheres { get; private set; }
        public int QuantidadeDeDiarias { get; private set; }
        public float ValorDiaria { get; private set; }
        //   [ForeignKey("ApartamentosId")]
        public int ApartamentosId { get; private set; }
        public int TipoHospedagensId { get; private set; }
        public int CheckinsId { get; private set; }
        public int EmpresasId { get; private set; }
        public int MotivoViagensId { get; private set; }
        public Empresa Empresas { get; set; }
        public Apartamentos Apartamentos { get; set; }
        public MotivoViagem MotivoViagens { get; set; }
        public TipoHospedagem TipoHospedagens { get; set; }
        public Checkins Checkins { get; set; }

        public ICollection<FacturaDividida> FacturaDivididas { get; set; }
        public ICollection<Historico> Historicos { get; set; }
        public ICollection<TransferenciaQuarto> TransferenciasOrigem { get; set; } = new List<TransferenciaQuarto>();
        public ICollection<TransferenciaQuarto> TransferenciasDestino { get; set; } = new List<TransferenciaQuarto>();

       // public ICollection<Reserva> Reservas { get; set; }

        public Hospedagem()
        {

        }

        public Hospedagem(DateTime dataAbertura, DateTime previsaoFechamento, float valorDiaria,
                          int apartamentosId, int quantidadeHomens,
                          int quantidadeMulheres, int quantidadeCrianca, int tipoHospedagensId, int empresasId, int motivoViagensId, int checkinsId)
        {
            if (dataAbertura >= previsaoFechamento)
                throw new ArgumentException("A data de abertura deve ser anterior à previsão de fechamento.");

            TimeSpan ts = previsaoFechamento.Date - dataAbertura.Date;
            var angolaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");

            var dataInicialAngola = TimeZoneInfo.ConvertTimeFromUtc(dataAbertura.ToUniversalTime(), angolaTimeZone).Date;
            var dataFinalAngola   = TimeZoneInfo.ConvertTimeFromUtc(previsaoFechamento.ToUniversalTime(), angolaTimeZone).Date;

          
            var dataEntradaNormalizada = new DateTime(dataAbertura.Year, dataAbertura.Month, dataAbertura.Day, 0, 0, 0, DateTimeKind.Unspecified);

            var dataPrevisaoNormalizada = new DateTime(previsaoFechamento.Year, previsaoFechamento.Month, previsaoFechamento.Day, 0, 0, 0, DateTimeKind.Unspecified);

            // 3. Calcular a diferença de dias
            var totalDias = (previsaoFechamento.Date - dataAbertura.Date).Days;


            Descricao = "";
            DataAbertura = dataAbertura.Date;  //dataEntradaNormalizada;
            QuantidadeDeDiarias = totalDias; //  (previsaoFechamento-dataAbertura).Days;;
            ValorDiaria = valorDiaria;
            ApartamentosId = apartamentosId;
            DateCreated = DateTime.Now;
            IsActive = true;
            DataFechamento = null;
            PrevisaoFechamento = previsaoFechamento.Date; //dataPrevisaoNormalizada;  //previsaoFechamento;
            DiariaAntecipada = false;
            EarlyCheckin = false;
            QuantidadeHomens = quantidadeHomens;
            QuantidadeMulheres = quantidadeMulheres;
            QuantidadeCrianca = quantidadeCrianca;
            TipoHospedagensId = tipoHospedagensId;
            EmpresasId = empresasId;
            MotivoViagensId = motivoViagensId;
            CheckinsId = checkinsId;
            Checkins = Checkins;
            //  Hospedes = new List<Hospede>();
            // Pagamentos = new List<Pagamento>();
        }

        public void update(int Id, DateTime dataAbertura, DateTime previsaoFechamento, float valorDiaria,
                      int apartamentosId, int quantidadeHomens,
                      int quantidadeMulheres, int quantidadeCrianca, int tipoHospedagensId, int empresasId, int motivoViagensId, int checkinsId)
        {
            TimeSpan ts = previsaoFechamento - dataAbertura;
            var angolaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");

            var dataInicialAngola = TimeZoneInfo.ConvertTimeFromUtc(dataAbertura.ToUniversalTime(), angolaTimeZone).Date;
            var dataFinalAngola   = TimeZoneInfo.ConvertTimeFromUtc(previsaoFechamento.ToUniversalTime(), angolaTimeZone).Date;
 var dataEntradaNormalizada = new DateTime(dataAbertura.Year, dataAbertura.Month, dataAbertura.Day, 0, 0, 0, DateTimeKind.Unspecified);

            var dataPrevisaoNormalizada = new DateTime(previsaoFechamento.Year, previsaoFechamento.Month, previsaoFechamento.Day, 0, 0, 0, DateTimeKind.Unspecified);

            // 3. Calcular a diferença de dias
            var totalDias = (previsaoFechamento.Date - dataAbertura.Date).Days;


            QuantidadeDeDiarias = totalDias;  //(previsaoFechamento.Date - dataAbertura.Date).Days; //ts.Days; //
            ValorDiaria = valorDiaria;
            ApartamentosId = apartamentosId;
            DateCreated = DateTime.Now;

            PrevisaoFechamento = previsaoFechamento.Date;  //previsaoFechamento;

            QuantidadeHomens = quantidadeHomens;
            QuantidadeMulheres = quantidadeMulheres;
            QuantidadeCrianca = quantidadeCrianca;
            TipoHospedagensId = tipoHospedagensId;
            EmpresasId = empresasId;
            MotivoViagensId = motivoViagensId;
            CheckinsId = checkinsId;
            Checkins = Checkins;
            //  Hospedes = new List<Hospede>();
            // Pagamentos = new List<Pagamento>();
        }

        public void FecharHospedagem(DateTime dataFechamento)
        {
            if (dataFechamento < DataAbertura)
                throw new ArgumentException("A data de fechamento não pode ser anterior à data de abertura.");

            DataFechamento = dataFechamento;
        }

        public void AtualizarDescricao(string descricao)
        {
            Descricao = descricao;
            LastModifiedDate = DateTime.Now;
        }
        public void AtualizaNovoApartamento(int apartamentoId)
        {
            ApartamentosId = apartamentoId;
            LastModifiedDate = DateTime.Now;
        }

        public float CalcularValorTotalHospedagem()
        {
            if (PrevisaoFechamento != DateTime.MinValue)
                QuantidadeDeDiarias = (PrevisaoFechamento.Date - DataAbertura.Date).Days;

            return QuantidadeDeDiarias * ValorDiaria;
        }

        /* public void AdicionarHospede(Hospede hospede)
            {
                if (Hospedes.Contains(hospede))
                    throw new InvalidOperationException("Hospede já está registrado nesta hospedagem.");

                Hospedes.Add(hospede);
                LastModifiedDate = DateTime.Now;
            } */

        /*  public void RegistrarPagamento(Pagamento pagamento)
         {
             if (pagamento == null)
                 throw new ArgumentException("O pagamento deve ser informado.");
             Pagamentos.Add(pagamento);
         } */
        public void AtualizarQuantidadeDeDiarias(int novaQuantidade)
        {
            if (novaQuantidade < 0)
                throw new ArgumentException("A quantidade de diárias não pode ser negativa.");

            QuantidadeDeDiarias = novaQuantidade;
        }
        public void AtualizarPrevisaoFechamento(DateTime novaData)
        {
            if (novaData < DataAbertura)
                throw new ArgumentException("A nova data de fechamento não pode ser anterior à data de abertura.");

            PrevisaoFechamento = novaData;
            CalcularValorTotalHospedagem();
        }

    }

}