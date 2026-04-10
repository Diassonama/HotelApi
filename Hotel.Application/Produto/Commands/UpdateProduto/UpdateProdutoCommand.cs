using Hotel.Application.Produto.Base;

namespace Hotel.Application.Produto.Commands.UpdateProduto
{
    public class UpdateProdutoCommand : ProdutoCommandBase
    {
        public int Id { get; set; }
    }
}