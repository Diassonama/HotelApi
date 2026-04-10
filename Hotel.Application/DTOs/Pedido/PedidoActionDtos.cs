using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.Pedido
{
    public class AddItemPedidoDto
    {
        [Required(ErrorMessage = "ID do produto é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "ID do produto deve ser maior que zero")]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int Quantidade { get; set; } = 1;
    }

    public class UpdateQuantityItemDto
    {
        [Required(ErrorMessage = "ID do produto é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "ID do produto deve ser maior que zero")]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "Nova quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int NovaQuantidade { get; set; }
    }

    public class UpdateObservacaoDto
    {
        [StringLength(500, ErrorMessage = "Observação não pode exceder 500 caracteres")]
        public string? Observacao { get; set; }
    }

    public class EstornoPedidoDto
    {
        [Required(ErrorMessage = "Motivo do estorno é obrigatório")]
        [StringLength(500, ErrorMessage = "Motivo não pode exceder 500 caracteres")]
        public string Motivo { get; set; } = string.Empty;
    }

    public class DescontoCalculadoDto
    {
        public int PedidoId { get; set; }
        public decimal ValorOriginal { get; set; }
        public decimal PercentualDesconto { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorComDesconto { get; set; }
    }
}