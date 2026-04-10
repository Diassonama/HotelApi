namespace Hotel.Domain.DTOs
{
    public class FacturaEmpresaDetalhesDto
    {
        public int Id { get; set; }
        public int EmpresasId { get; set; }
        public int CheckinsId { get; set; }
        public float Total { get; set; }
        public float ValorDiaria { get; set; }
       // public DateTime DataFactura { get; set; }
        public string NumeroFactura { get; set; }

        // Empresa
        public string RazaoSocialEmpresa { get; set; }

        // Cliente Principal
        public string NomeCliente { get; set; }
        public string EmailCliente { get; set; }

        // Checkin
        public DateTime DataEntrada { get; set; }
        public DateTime? DataSaida { get; set; }

        // Apartamento
        public string Quarto { get; set; }
       // public string NumeroApartamento { get; set; }

        // Totais

        public int QuantidadeHospedes { get; set; }
        public int QuantidadeDeDiarias { get; set; }
         public string Hospede { get; set; }
         public string situacaoFacturas { get; set; } = string.Empty;
    }
}