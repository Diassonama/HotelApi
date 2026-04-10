using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.TipoApartamento
{
    /// <summary>
    /// Request para atualizar um tipo de apartamento
    /// </summary>
    public class UpdateTipoApartamentoRequest
    {
        [Required(ErrorMessage = "ID é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "ID deve ser maior que zero")]
        public int Id { get; set; }

        
        [StringLength(500, ErrorMessage = "Descrição deve ter no máximo 500 caracteres")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Valor da diária single é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor da diária single deve ser maior que zero")]
        public decimal ValorDiariaSingle { get; set; }

        [Required(ErrorMessage = "Valor da diária double é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor da diária double deve ser maior que zero")]
        public decimal ValorDiariaDouble { get; set; }

        [Required(ErrorMessage = "Valor da diária triple é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor da diária triple deve ser maior que zero")]
        public decimal ValorDiariaTriple { get; set; }

        [Required(ErrorMessage = "Valor da diária quádruple é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor da diária quádruple deve ser maior que zero")]
        public decimal ValorDiariaQuadruple { get; set; }

        [Required(ErrorMessage = "Valor de uma hora é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor de uma hora deve ser maior que zero")]
        public decimal ValorUmaHora { get; set; }

        [Required(ErrorMessage = "Valor de duas horas é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor de duas horas deve ser maior que zero")]
        public decimal ValorDuasHora { get; set; }

        [Required(ErrorMessage = "Valor de três horas é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor de três horas deve ser maior que zero")]
        public decimal ValortresHora { get; set; }

        [Required(ErrorMessage = "Valor de quatro horas é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor de quatro horas deve ser maior que zero")]
        public decimal ValorQuatroHora { get; set; }

        [Required(ErrorMessage = "Valor da noite é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor da noite deve ser maior que zero")]
        public decimal ValorNoite { get; set; }

        // Valores especiais por dia da semana
        [Range(0, double.MaxValue, ErrorMessage = "Valor de domingo deve ser maior ou igual a zero")]
        public decimal? Domingo { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Valor de segunda deve ser maior ou igual a zero")]
        public decimal? Segunda { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Valor de terça deve ser maior ou igual a zero")]
        public decimal? Terca { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Valor de quarta deve ser maior ou igual a zero")]
        public decimal? Quarta { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Valor de quinta deve ser maior ou igual a zero")]
        public decimal? Quinta { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Valor de sexta deve ser maior ou igual a zero")]
        public decimal? Sexta { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Valor de sábado deve ser maior ou igual a zero")]
        public decimal? Sabado { get; set; }

        public bool Ativo { get; set; } = true;
    }
}