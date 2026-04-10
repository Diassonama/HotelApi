using System;

namespace Hotel.Application.Dtos
{
    public class ReciboCheckoutDto
    {
        // ✅ DADOS DA HOTELARIA
        public string NomeHotel { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string NumContribuinte { get; set; }

        // ✅ DADOS DO CHECK-IN
        public int CheckinNumero { get; set; }
        public string NomeHospede { get; set; }
        public string ApartamentoCodigo { get; set; }
        public string TipoApartamento { get; set; }

        // ✅ PERÍODO DE HOSPEDAGEM
        public DateTime DataEntrada { get; set; }
        public DateTime DataSaida { get; set; }
        public int NumDias { get; set; }
        public DateTime DataImpressao { get; set; }

        // ✅ VALORES
        public float ValorDiaria { get; set; }
        public float ValorDiarias { get; set; }
        public float Consumo { get; set; }
        public float Desconto { get; set; }
        public float Total { get; set; }
        public float Pago { get; set; }
        public float APagar { get; set; }

        // ✅ DADOS ADICIONAIS
        public string Operador { get; set; }
        public string FormaPagamento { get; set; }
        public string DecretoFiscal { get; set; }
        public string TipoHospede { get; set; } // PASSANTE, EMPRESA, etc.
    }
}