using System.ComponentModel.DataAnnotations;

namespace Hotel.Application.DTOs.Pedido
{
    public class CreatePedidoDto
    {
        [Required(ErrorMessage = "ID do checkin é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "ID do checkin deve ser maior que zero")]
        public int IdCheckin { get; set; }

        [Required(ErrorMessage = "ID do hóspede é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "ID do hóspede deve ser maior que zero")]
        public int HospedeId { get; set; }

        [Required(ErrorMessage = "ID do ponto de venda é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "ID do ponto de venda deve ser maior que zero")]
        public int PontoVendaId { get; set; }

        [StringLength(500, ErrorMessage = "Observação não pode exceder 500 caracteres")]
        public string? Observacao { get; set; }

        public List<CreateItemPedidoDto>? Itens { get; set; }
    }

    public class CreateItemPedidoDto
    {
        [Required(ErrorMessage = "ID do produto é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "ID do produto deve ser maior que zero")]
        public int ProdutoId { get; set; }

        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "Quantidade é obrigatória")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }
    }
}