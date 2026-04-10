using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Produtos : BaseDomainEntity
    {
        private const int NOME_MAX_LENGTH = 100;
        private const int CAMINHO_IMAGEM_MAX_LENGTH = 500;
        private const float MIN_VALOR = 0.01f;
        private const float MIN_PRECO_COMPRA = 0.01f;
        private const int MIN_ESTOQUE = 0;

        // ✅ PROPRIEDADES COM TIPOS CORRETOS (FLOAT para compatibilidade com banco)
        public string Nome { get; set; }
        public float Valor { get; set; }  // ✅ FLOAT conforme banco
        public int Quantidade { get; set; }
        public int EstoqueMinimo { get; set; }
        public int AdicionarStock { get; set; }
        public float Lucro { get; set; }  // ✅ FLOAT conforme banco
        public float MargemLucro { get; set; }  // ✅ FLOAT conforme banco
        public DateTime DataExpiracao { get; set; }
        public float ValorFixo { get; set; }  // ✅ FLOAT conforme banco
        public float PrecoCompra { get; set; }  // ✅ FLOAT conforme banco
        public float Desconto { get; set; }  // ✅ FLOAT conforme banco
        public float DescontoPercentagem { get; set; }  // ✅ FLOAT conforme banco
        public int PontoDeVendasId { get; set; }
        public string CaminhoImagem { get; set; }
        public int CategoriaId { get; set; }
        public float PrecoCIva { get; set; }  // ✅ FLOAT conforme banco
        public string ProductTypeCode { get; set; }
        public int TaxTableEntryId { get; set; }
        public string TaxExemptionReasonCode { get; set; }

        // ✅ PROPRIEDADES DE NAVEGAÇÃO
        public virtual Categoria Categoria { get; set; }
        public virtual PontoDeVenda PontoDeVenda { get; set; }
        public virtual ProductType ProductTypes { get; set; }
        public virtual TaxTableEntry TaxTableEntry { get; set; }
        public virtual TaxExemptionReason TaxExemptionReason { get; set; }
        public virtual ICollection<ProdutoStock> ProdutoStocks { get; set; }

        // ✅ PROPRIEDADES CALCULADAS
        public bool EstaExpirado => DateTime.Now > DataExpiracao;
        public bool EstoqueAbaixoDoMinimo => Quantidade <= EstoqueMinimo;
        public bool EstaDisponivel => Quantidade > 0 && !EstaExpirado;
        public int DiasParaExpiracao => (DataExpiracao - DateTime.Now).Days;
        public float LucroUnitario => Valor - PrecoCompra;
        public float PercentualLucroReal => PrecoCompra > 0 ? (LucroUnitario / PrecoCompra) * 100 : 0;

        // ✅ CONSTRUTOR PADRÃO
        public Produtos() 
        {
            ProdutoStocks = new List<ProdutoStock>();
        }

        // ✅ CONSTRUTOR COM PARÂMETROS
        public Produtos(
            string nome, 
            float valor, 
            float precoCompra, 
            int categoriaId,
            int pontoDeVendasId, 
            DateTime dataExpiracao, 
            string productTypeCode, 
            string taxExemptionReasonCode,
            int taxTableEntryId,
            float margemLucro, 
            int quantidade, 
            int estoqueMinimo, 
            int adicionarStock, 
            float lucro, 
            float valorFixo,
            float precoCIva, 
            float desconto, 
            float descontoPercentagem)
        {
            Nome = nome;
            Valor = valor;
            PrecoCompra = precoCompra;
            CategoriaId = categoriaId;
            PontoDeVendasId = pontoDeVendasId;
            DataExpiracao = dataExpiracao;
            ProductTypeCode = productTypeCode;
            TaxExemptionReasonCode = taxExemptionReasonCode;
            TaxTableEntryId = taxTableEntryId;
            MargemLucro = margemLucro;
            Quantidade = quantidade;
            EstoqueMinimo = estoqueMinimo;
            AdicionarStock = adicionarStock;
            Lucro = lucro;
            ValorFixo = valorFixo;
            PrecoCIva = precoCIva;
            Desconto = desconto;
            DescontoPercentagem = descontoPercentagem;
            
            ProdutoStocks = new List<ProdutoStock>();
        }
    }
}