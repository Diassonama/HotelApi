using System;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class ItemPedido : BaseDomainEntity
    {
        // Construtor privado para EF Core
        private ItemPedido() { }

        // Construtor para criação de novos itens
        public ItemPedido(int produtoId, string nomeProduto, decimal precoUnitario, int quantidade, string observacao = null, string categoria = null)
        {
            if (produtoId <= 0) throw new ArgumentException("ID do produto deve ser maior que zero", nameof(produtoId));
            if (string.IsNullOrWhiteSpace(nomeProduto)) throw new ArgumentException("Nome do produto é obrigatório", nameof(nomeProduto));
            if (precoUnitario <= 0) throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(precoUnitario));
            if (quantidade <= 0) throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

            ProdutoId = produtoId;
            NomeProduto = nomeProduto.Trim();
            PrecoUnitario = precoUnitario;
            Quantidade = quantidade;
            Observacao = observacao?.Trim();
            Categoria = categoria?.Trim();
        }

        // Propriedades
        public int ProdutoId { get; private set; }
        public string NomeProduto { get; private set; }
        public decimal PrecoUnitario { get; private set; }
        public int Quantidade { get; private set; }
        public string Observacao { get; private set; }
        public string Categoria { get; private set; }

        // Propriedades calculadas
        public decimal ValorTotal => PrecoUnitario * Quantidade;

        // Navigation Properties
        public int PedidoId { get; private set; }
        public virtual Pedido Pedido { get; private set; }

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

        public void AtualizarObservacao(string novaObservacao)
        {
            Observacao = novaObservacao?.Trim();
        }

        public void AtualizarPreco(decimal novoPreco)
        {
            if (novoPreco <= 0) throw new ArgumentException("Preço deve ser maior que zero", nameof(novoPreco));
            PrecoUnitario = novoPreco;
        }

        public override string ToString()
        {
            return $"{NomeProduto} - Qtd: {Quantidade} - R$ {ValorTotal:F2}";
        }
    }
}