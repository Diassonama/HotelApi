using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.TipoHospedagem
{
    /// <summary>
    /// Request para atualizar um tipo de hospedagem
    /// </summary>
    public class UpdateTipoHospedagemRequest
    {
        [Required(ErrorMessage = "ID é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "ID deve ser maior que zero")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Descrição deve ter entre 3 e 100 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, float.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public float Valor { get; set; }

        public bool Ativo { get; set; } = true;
    }
}