using System;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class ItemPedido : BaseDomainEntity
    {
        // Construtor privado para EF Core
        private ItemPedido() { }

        // Construtor para criação de novos itens
        public ItemPedido(int produtoId, decimal preco, int quantidade)
        {
            if (produtoId <= 0) throw new ArgumentException("ID do produto deve ser maior que zero", nameof(produtoId));
            if (preco <= 0) throw new ArgumentException("Preço deve ser maior que zero", nameof(preco));
            if (quantidade <= 0) throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

            ProdutoId = produtoId;
            Preco = preco;
            Quantidade = quantidade;
        }

        // Propriedades
        public int ProdutoId { get; private set; }
        public decimal Preco { get; private set; }
        public int Quantidade { get; private set; }

        // Propriedades calculadas
        public decimal ValorTotal => Preco * Quantidade;

        // Navigation Properties
        public int PedidoId { get; private set; }
        public virtual Pedido Pedido { get; private set; }
        public virtual Produtos Produto { get; private set; }

        // Métodos de negócio
        public void AlterarQuantidade(int novaQuantidade)
        {
            if (novaQuantidade <= 0) throw new ArgumentException("Quantidade deve ser maior que zero", nameof(novaQuantidade));
            Quantidade = novaQuantidade;
        }

        public void AdicionarQuantidade(int quantidadeAdicional)
        {
            if (quantidadeAdicional <= 0) throw new ArgumentException("Quantidade adicional deve ser maior que zero", nameof(quantidadeAdicional));
            Quantidade += quantidadeAdicional;
        }

        public void AtualizarPreco(decimal novoPreco)
        {
            if (novoPreco <= 0) throw new ArgumentException("Preço deve ser maior que zero", nameof(novoPreco));
            Preco = novoPreco;
        }

        public override string ToString()
        {
            return $"Produto {ProdutoId} - Qtd: {Quantidade} - R$ {ValorTotal:F2}";
        }
    }
}