using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.TipoHospedagem
{
    /// <summary>
    /// Request para criar um novo tipo de hospedagem
    /// </summary>
    public class CreateTipoHospedagemRequest
    {
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Descrição deve ter entre 3 e 250 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, float.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public float Valor { get; set; }
    }
}