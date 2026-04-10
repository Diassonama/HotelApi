using System;
using System.Collections.Generic;
using System.Linq;
using Hotel.Domain.Common;
using Hotel.Domain.Enums;

namespace Hotel.Domain.Entities
{
    public class Pedido : BaseDomainEntity
    {
        private readonly List<ItemPedido> _itemPedidos;

        // Construtor privado para EF Core
        private Pedido()
        {
            _itemPedidos = new List<ItemPedido>();
            SituacaoPagamento = SituacaoPagamentoPedido.Pendente;
            DataPedido = DateTime.Now;
            RecalcularValorTotal();
        }

        // Construtor para criação de novos pedidos
        public Pedido(int idCaixa, int idCheckin, int hospedeId, int pontoVendaId, string observacao = null)
        {
            if (idCaixa <= 0) throw new ArgumentException("ID do caixa deve ser maior que zero", nameof(idCaixa));
            if (idCheckin <= 0) throw new ArgumentException("ID do checkin deve ser maior que zero", nameof(idCheckin));
            if (hospedeId <= 0) throw new ArgumentException("ID do hóspede deve ser maior que zero", nameof(hospedeId));
            if (pontoVendaId <= 0) throw new ArgumentException("ID do ponto de venda deve ser maior que zero", nameof(pontoVendaId));

            IdCaixa = idCaixa;
            IdCheckin = idCheckin;
            HospedeId = hospedeId;
            PontoVendaId = pontoVendaId;
            Observacao = observacao?.Trim();
            
            _itemPedidos = new List<ItemPedido>();
            SituacaoPagamento = SituacaoPagamentoPedido.Pendente;
            DataPedido = DateTime.Now;
            NumePedido = GerarNumeroPedido();
            RecalcularValorTotal();
        }

        // Propriedades públicas (somente leitura quando apropriado)
        public int IdCaixa { get; private set; }
        public int IdCheckin { get; private set; }
        public int HospedeId { get; private set; }
        public int PontoVendaId { get; private set; }
        public string NumePedido { get; private set; }
        public string Observacao { get; private set; }
        public DateTime DataPedido { get; private set; }
        public DateTime? DataFinalizacao { get; private set; }
        public SituacaoPagamentoPedido SituacaoPagamento { get; private set; }
        public decimal Valor { get; private set; }
        
        // Propriedades calculadas
        public decimal ValorTotal => Valor;
        public int QuantidadeItens => _itemPedidos.Count;
        public bool PedidoFinalizado => SituacaoPagamento == SituacaoPagamentoPedido.Pago;
        public bool PodeCancelar => SituacaoPagamento == SituacaoPagamentoPedido.Pendente;

        // Navigation Properties
        public virtual PontoDeVenda PontoVenda { get; private set; }
        public virtual Hospede Hospede { get; private set; }
        public virtual IReadOnlyCollection<ItemPedido> ItemPedidos => _itemPedidos.AsReadOnly();

        // Métodos de negócio
        public void AdicionarItem(int produtoId, string nomeProduto, decimal precoUnitario, int quantidade = 1, string observacaoItem = null)
        {
            ValidarPedidoParaEdicao();
            
            if (produtoId <= 0) throw new ArgumentException("ID do produto deve ser maior que zero", nameof(produtoId));
            if (string.IsNullOrWhiteSpace(nomeProduto)) throw new ArgumentException("Nome do produto é obrigatório", nameof(nomeProduto));
            if (precoUnitario <= 0) throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(precoUnitario));
            if (quantidade <= 0) throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));

            // Verificar se já existe item com mesmo produto
            var itemExistente = _itemPedidos.FirstOrDefault(i => i.ProdutoId == produtoId);
            
            if (itemExistente != null)
            {
                itemExistente.AdicionarQuantidade(quantidade);
            }
            else
            {
                var novoItem = new ItemPedido(produtoId, nomeProduto, precoUnitario, quantidade, observacaoItem);
                _itemPedidos.Add(novoItem);
            }

            RecalcularValorTotal();
        }

        public void RemoverItem(int produtoId)
        {
            ValidarPedidoParaEdicao();
            
            var item = _itemPedidos.FirstOrDefault(i => i.ProdutoId == produtoId);
            if (item == null) throw new InvalidOperationException("Item não encontrado no pedido");
            
            _itemPedidos.Remove(item);
            RecalcularValorTotal();
        }

        public void AlterarQuantidadeItem(int produtoId, int novaQuantidade)
        {
            ValidarPedidoParaEdicao();
            
            if (novaQuantidade <= 0) throw new ArgumentException("Quantidade deve ser maior que zero", nameof(novaQuantidade));
            
            var item = _itemPedidos.FirstOrDefault(i => i.ProdutoId == produtoId);
            if (item == null) throw new InvalidOperationException("Item não encontrado no pedido");
            
            item.AlterarQuantidade(novaQuantidade);
            RecalcularValorTotal();
        }

        public void AtualizarObservacao(string novaObservacao)
        {
            ValidarPedidoParaEdicao();
            Observacao = novaObservacao?.Trim();
        }

        public void ConfirmarPagamento()
        {
            if (SituacaoPagamento == SituacaoPagamentoPedido.Pago)
                throw new InvalidOperationException("Pedido já foi pago");
            
            if (!_itemPedidos.Any())
                throw new InvalidOperationException("Não é possível confirmar pagamento de pedido sem itens");

            SituacaoPagamento = SituacaoPagamentoPedido.Pago;
            DataFinalizacao = DateTime.Now;
        }

        public void CancelarPedido()
        {
            if (!PodeCancelar)
                throw new InvalidOperationException("Pedido não pode ser cancelado");

            SituacaoPagamento = SituacaoPagamentoPedido.Cancelado;
            DataFinalizacao = DateTime.Now;
        }

        public void EstornarPagamento(string motivo)
        {
            if (SituacaoPagamento != SituacaoPagamentoPedido.Pago)
                throw new InvalidOperationException("Só é possível estornar pedidos pagos");
            
            if (string.IsNullOrWhiteSpace(motivo))
                throw new ArgumentException("Motivo do estorno é obrigatório", nameof(motivo));

            SituacaoPagamento = SituacaoPagamentoPedido.Estornado;
            Observacao = $"{Observacao} | ESTORNO: {motivo}".Trim('|', ' ');
        }

        // Métodos de consulta
        public decimal CalcularDesconto(decimal percentualDesconto)
        {
            if (percentualDesconto < 0 || percentualDesconto > 100)
                throw new ArgumentException("Percentual de desconto deve estar entre 0 e 100", nameof(percentualDesconto));
            
            return ValorTotal * (percentualDesconto / 100);
        }

        public IEnumerable<ItemPedido> ObterItensPorCategoria(string categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria)) return Enumerable.Empty<ItemPedido>();
            
            return _itemPedidos.Where(i => i.Categoria?.Equals(categoria, StringComparison.OrdinalIgnoreCase) == true);
        }

        public bool TemProduto(int produtoId)
        {
            return _itemPedidos.Any(i => i.ProdutoId == produtoId);
        }

        // Métodos privados auxiliares
        private void ValidarPedidoParaEdicao()
        {
            if (SituacaoPagamento != SituacaoPagamentoPedido.Pendente)
                throw new InvalidOperationException("Não é possível editar pedido que não está pendente");
        }

        private string GerarNumeroPedido()
        {
            return $"PED{DateTime.Now:yyyyMMdd}{DateTime.Now:HHmmss}";
        }

        private void RecalcularValorTotal()
        {
            Valor = _itemPedidos.Sum(item => item.ValorTotal);
        }

        // Override para melhor representação
        public override string ToString()
        {
            return $"Pedido {NumePedido} - {SituacaoPagamento} - R$ {ValorTotal:F2}";
        }
    }

    // Enum para situação do pagamento
    public enum SituacaoPagamentoPedido
    {
        Pendente = 1,
        Pago = 2,
        Cancelado = 3,
        Estornado = 4
    }
}