using System;
using System.Collections.Generic;
using System.Linq;
using Hotel.Domain.Common;


namespace Hotel.Domain.Entities
{
    public class Produto : BaseDomainEntity
    {
        // Constantes de validação
        private const int NOME_MAX_LENGTH = 100;
        private const int CAMINHO_IMAGEM_MAX_LENGTH = 500;
        private const float MIN_VALOR = 0.01f;
        private const float MIN_PRECO_COMPRA = 0.01f;
        private const int MIN_ESTOQUE = 0;

		// Campos privados para encapsulamento
		/*    private string _nome;
		   private float _valor;
		   private int _quantidade;
		   private int _estoqueMinimo;
		   private float _precoCompra;
		   private float _margemLucro;
		   private float _desconto;
		   private float _descontoPercentagem;
		   private DateTime _dataExpiracao;
		   private string _caminhoImagem;
		   private List<ProdutoStock> _produtoStocks; */

		// Construtor privado para Entity Framework
		private Produto()
		{
			ProdutoStocks = new List<ProdutoStock>();
			Desconto = 0;
            DescontoPercentagem = 0;
            AdicionarStock = 0;
            Lucro = 0;
            ValorFixo = 0;
            PrecoCIva = 0;
        }

        // Construtor para criação de novo produto
        public Produto(
			string nome,
			float valor,
			float precoCompra,
			int categoriaId,
			int pontoDeVendasId,
			DateTime dataExpiracao,
			string productTypeCode,
			string taxExemptionReasonCode ,
			int taxTableEntryId,
			float margemLucro = 0,
			int quantidade  = 0,
			int estoqueMinimo = 0,
			int adicionarStock  = 0,

			float lucro  = 0,
			float valorFixo  = 0,
			float precoCIva  = 0,
			float desconto  = 0,
			float descontoPercentagem  = 0,


			string caminhoImagem = null) : this()
        {
            ValidarDadosObrigatorios(nome, valor, precoCompra, categoriaId, pontoDeVendasId, dataExpiracao, productTypeCode, taxTableEntryId, taxExemptionReasonCode);
            
            SetNome(nome);
            SetValor(valor);
            SetPrecoCompra(precoCompra);
            SetCategoriaId(categoriaId);
            SetPontoDeVendasId(pontoDeVendasId);
            SetDataExpiracao(dataExpiracao);
            SetMargemLucro(margemLucro);
            SetQuantidade(quantidade);
            SetEstoqueMinimo(estoqueMinimo);
            SetCaminhoImagem(caminhoImagem);
            SetProductTypeCode(productTypeCode);
            SetTaxTableEntryId(taxTableEntryId);
            SetTaxExemptionReasonCode(taxExemptionReasonCode);
            
			
			  // ✅ Definir propriedades que estavam faltando
            AdicionarStock = adicionarStock; // ✅ Uso do parâmetro adicionarStock
            Desconto = desconto;             // ✅ Uso do parâmetro desconto
            DescontoPercentagem = descontoPercentagem; // ✅ Uso do parâmetro descontoPercentagem
            
			Lucro = lucro != 0 ? lucro : (valor - precoCompra);
            ValorFixo = valorFixo != 0 ? valorFixo : valor;

            // ✅ Calcular ou definir valores derivados
			 if (precoCIva != 0)
                PrecoCIva = precoCIva;
            else
              //  CalcularPrecoComIva();
            CalcularValoresDerivados();
        }

        // Propriedades públicas somente leitura
        public string Nome { get; set; }
        public float Valor { get; set; }
       // public int Quantidade => _quantidade;
		 public int Quantidade { get; set; }
        public int EstoqueMinimo { get; set; }
        public int AdicionarStock { get;  set; }
        public float Lucro { get;  set; }
        public float MargemLucro { get; set; }
        public DateTime DataExpiracao { get; set; }
        public float ValorFixo { get;  set; }
        public float PrecoCompra { get; set; }
        public float Desconto { get; set; }
        public float DescontoPercentagem { get; set; }
        public int PontoDeVendasId { get;  set; }
        public string CaminhoImagem { get; set; }
        public int CategoriaId { get;  set; }
        public float PrecoCIva { get;  set; }
		public string ProductTypeCode  { get;  set; }
		public int TaxTableEntryId { get;  set; }
		public string TaxExemptionReasonCode { get;  set; }

        // Navigation Properties
		public Categoria Categoria { get; private set; }
        public PontoDeVenda PontoDeVenda { get; private set; }
        public ProductType ProductTypes { get; private set; }
        public TaxTableEntry TaxTableEntry { get; private set; }
        public TaxExemptionReason TaxExemptionReason { get; private set; }
        public ICollection<ProdutoStock> ProdutoStocks { get; set; }
       // private List<ProdutoStock> ProdutoStocks { get; set; }

		
      //  public list<ProdutoStock> ProdutoStocks => _produtoStocks.AsReadOnly();

		// Propriedades calculadas
		public bool EstaExpirado => DateTime.Now > DataExpiracao;
        public bool EstoqueAbaixoDoMinimo => Quantidade <= EstoqueMinimo;
        public bool EstaDisponivel => Quantidade > 0 && !EstaExpirado;
        public int DiasParaExpiracao => (DataExpiracao - DateTime.Now).Days;
        public float LucroUnitario => Valor - PrecoCompra;
        public float PercentualLucroReal => PrecoCompra > 0 ? (LucroUnitario / PrecoCompra) * 100 : 0;

        // Métodos de comportamento de negócio

        /// <summary>
        /// Atualiza as informações básicas do produto
        /// </summary>
        public void AtualizarInformacoes(
            string nome,
            float valor,
            float precoCompra,
            float margemLucro,
            DateTime dataExpiracao,
            string caminhoImagem = null)
        {
            ValidarDadosBasicos(nome, valor, precoCompra, dataExpiracao);
            
            SetNome(nome);
            SetValor(valor);
            SetPrecoCompra(precoCompra);
            SetMargemLucro(margemLucro);
            SetDataExpiracao(dataExpiracao);
            SetCaminhoImagem(caminhoImagem);
            
            CalcularValoresDerivados();
            AtualizarDataModificacao();
        }

        /// <summary>
        /// Adiciona quantidade ao estoque
        /// </summary>
       /*  public void AdicionarEstoque(int quantidade, string observacao = null)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade a adicionar deve ser maior que zero.");

            if (EstaExpirado)
                throw new ArgumentException("Não é possível adicionar estoque a um produto expirado.");

            var novaQuantidade = _quantidade + quantidade;
            SetQuantidade(novaQuantidade);
            
            AdicionarStock = quantidade;
            AtualizarDataModificacao();
            
            // Adicionar registro de movimentação de estoque
            AdicionarMovimentacaoEstoque(quantidade, "ENTRADA", observacao ?? $"Entrada de {quantidade} unidades");
        }
 */

 public void AdicionarEstoque(int quantidadeAdicionar, string observacao = null)
        {
            if (quantidadeAdicionar <= 0)
                throw new ArgumentException("Quantidade a adicionar deve ser maior que zero.");

            Quantidade += quantidadeAdicionar;
            AdicionarStock = quantidadeAdicionar;
            LastModifiedDate = DateTime.UtcNow;
        }
		/// <summary>
		/// Remove quantidade do estoque
		/// </summary>
		public void RemoverEstoque(int quantidade, string observacao = null)
		{
			if (quantidade <= 0)
				throw new ArgumentException("Quantidade a remover deve ser maior que zero.");

			if (quantidade > Quantidade)
				throw new ArgumentException($"Quantidade insuficiente em estoque. Disponível: {quantidade}, Solicitado: {quantidade}");

			if (EstaExpirado)
				throw new ArgumentException("Não é possível remover estoque de um produto expirado.");

			var novaQuantidade = Quantidade - quantidade;
			SetQuantidade(novaQuantidade);
			AtualizarDataModificacao();

			// Adicionar registro de movimentação de estoque
			AdicionarMovimentacaoEstoque(-quantidade, "SAIDA", observacao ?? $"Saída de {quantidade} unidades");
		}

        /// <summary>
        /// Ajusta o estoque para uma quantidade específica
        /// </summary>
        public void AjustarEstoque(int novaQuantidade, string observacao = null)
        {
            if (novaQuantidade < 0)
                throw new ArgumentException("Quantidade não pode ser negativa.");

            var quantidadeAnterior = Quantidade;
            SetQuantidade(novaQuantidade);
            AtualizarDataModificacao();
            
            var diferenca = novaQuantidade - quantidadeAnterior;
            var tipoMovimentacao = diferenca > 0 ? "AJUSTE_ENTRADA" : "AJUSTE_SAIDA";
            
            // Adicionar registro de movimentação de estoque
            AdicionarMovimentacaoEstoque(diferenca, tipoMovimentacao, 
                observacao ?? $"Ajuste de estoque de {quantidadeAnterior} para {novaQuantidade}");
        }

        /// <summary>
        /// Define o estoque mínimo
        /// </summary>
        public void DefinirEstoqueMinimo(int estoqueMinimo)
        {
            SetEstoqueMinimo(estoqueMinimo);
            AtualizarDataModificacao();
        }

        /// <summary>
        /// Aplica desconto em valor fixo
        /// </summary>
        public void AplicarDescontoFixo(float valorDesconto, string motivo = null)
        {
            if (valorDesconto < 0)
                throw new ArgumentException("Valor do desconto não pode ser negativo.");

            if (valorDesconto >= Valor)
                throw new ArgumentException("Desconto não pode ser maior ou igual ao valor do produto.");

            Desconto = valorDesconto;
            DescontoPercentagem = Desconto /Valor * 100;
            CalcularValoresDerivados();
            AtualizarDataModificacao();
        }

        /// <summary>
        /// Aplica desconto em percentual
        /// </summary>
        public void AplicarDescontoPercentual(float percentualDesconto, string motivo = null)
        {
            if (percentualDesconto < 0 || percentualDesconto > 100)
                throw new ArgumentException("Percentual de desconto deve estar entre 0 e 100.");

            DescontoPercentagem = percentualDesconto;
            Desconto = Valor * percentualDesconto / 100;
            CalcularValoresDerivados();
            AtualizarDataModificacao();
        }

        /// <summary>
        /// Remove desconto aplicado
        /// </summary>
        public void RemoverDesconto()
        {
            Desconto = 0;
            DescontoPercentagem = 0;
            CalcularValoresDerivados();
            AtualizarDataModificacao();
        }

        /// <summary>
        /// Verifica se o produto pode ser vendido
        /// </summary>
        public bool PodeSerVendido(int quantidadeDesejada = 1)
        {
            return !EstaExpirado && 
                   Quantidade >= quantidadeDesejada && 
                   quantidadeDesejada > 0;
        }

        /// <summary>
        /// Calcula o valor final com desconto
        /// </summary>
        public float CalcularValorFinal()
        {
            return Valor - Desconto;
        }

        /// <summary>
        /// Calcula o valor total do estoque
        /// </summary>
        public float CalcularValorTotalEstoque()
        {
            return CalcularValorFinal() * Quantidade;
        }

        /// <summary>
        /// Extende a data de expiração
        /// </summary>
        public void ExtenderDataExpiracao(int diasAdicionais, string motivo = null)
        {
            if (diasAdicionais <= 0)
                throw new ArgumentException("Dias adicionais deve ser maior que zero.");

            var novaDataExpiracao = DataExpiracao.AddDays(diasAdicionais);
            SetDataExpiracao(novaDataExpiracao);
            AtualizarDataModificacao();
        }

        // Métodos privados de validação e configuração

        private void ValidarDadosObrigatorios(
            string nome, 
            float valor, 
            float precoCompra, 
            int categoriaId, 
            int pontoDeVendasId, 
            DateTime dataExpiracao, string productTypeCode, int taxTableEntryId, string taxExemptionReasonCode)
        {
            ValidarDadosBasicos(nome, valor, precoCompra, dataExpiracao);
            
            if (categoriaId <= 0)
                throw new ArgumentException("Categoria é obrigatória.");

            if (string.IsNullOrWhiteSpace(productTypeCode))
                throw new ArgumentException("Tipo de produto é obrigatório.");

            if (taxTableEntryId < 0)
                throw new ArgumentException("Entrada da tabela de impostos é obrigatória.");

            if (string.IsNullOrWhiteSpace(taxExemptionReasonCode))
                throw new ArgumentException("Código de motivo de isenção é obrigatório.");
                
            if (pontoDeVendasId <= 0)
                throw new ArgumentException("Ponto de vendas é obrigatório.");
        }

        private void ValidarDadosBasicos(string nome, float valor, float precoCompra, DateTime dataExpiracao)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório.");
                
            if (valor < MIN_VALOR)
                throw new ArgumentException($"Valor deve ser maior que {MIN_VALOR:C}.");
                
            if (precoCompra < MIN_PRECO_COMPRA)
                throw new ArgumentException($"Preço de compra deve ser maior que {MIN_PRECO_COMPRA:C}.");
                
            if (dataExpiracao <= DateTime.Now)
                throw new ArgumentException("Data de expiração deve ser futura.");
        }

        private void SetNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório.");
                
            if (nome.Length > NOME_MAX_LENGTH)
                throw new ArgumentException($"Nome não pode exceder {NOME_MAX_LENGTH} caracteres.");
                
            Nome = nome.Trim();
        }

        private void SetValor(float valor)
        {
            if (valor < MIN_VALOR)
                throw new ArgumentException($"Valor deve ser maior que {MIN_VALOR:C}.");
                
            Valor = valor;
        }

        private void SetQuantidade(int quantidade)
        {
            if (quantidade < MIN_ESTOQUE)
                throw new ArgumentException($"Quantidade não pode ser menor que {MIN_ESTOQUE}.");
                
            Quantidade = quantidade;
        }

        private void SetEstoqueMinimo(int estoqueMinimo)
        {
            if (estoqueMinimo < MIN_ESTOQUE)
                throw new ArgumentException($"Estoque mínimo não pode ser menor que {MIN_ESTOQUE}.");
                
            EstoqueMinimo = estoqueMinimo;
        }

        private void SetPrecoCompra(float precoCompra)
        {
            if (precoCompra < MIN_PRECO_COMPRA)
                throw new ArgumentException($"Preço de compra deve ser maior que {MIN_PRECO_COMPRA:C}.");
                
            PrecoCompra = precoCompra;
        }

        private void SetMargemLucro(float margemLucro)
        {
            if (margemLucro < 0)
                throw new ArgumentException("Margem de lucro não pode ser negativa.");
                
            MargemLucro = margemLucro;
        }

        private void SetDataExpiracao(DateTime dataExpiracao)
        {
            if (dataExpiracao <= DateTime.Now)
                throw new ArgumentException("Data de expiração deve ser futura.");
                
            DataExpiracao = dataExpiracao;
        }

        private void SetCaminhoImagem(string caminhoImagem)
        {
            if (!string.IsNullOrWhiteSpace(caminhoImagem) && caminhoImagem.Length > CAMINHO_IMAGEM_MAX_LENGTH)
                throw new ArgumentException($"Caminho da imagem não pode exceder {CAMINHO_IMAGEM_MAX_LENGTH} caracteres.");
                
            CaminhoImagem = caminhoImagem?.Trim();
        }
        private void SetProductTypeCode(string productTypeCode)
        {
            if (string.IsNullOrWhiteSpace(productTypeCode))
                throw new ArgumentException("Tipo de produto é obrigatório.");

            ProductTypeCode = productTypeCode;
        }
        private void SetTaxTableEntryId(int taxTableEntryId)
        {
            if (taxTableEntryId < 0)
                throw new ArgumentException("Entrada da tabela de impostos é obrigatória.");

            TaxTableEntryId = taxTableEntryId;
        }
        private void SetTaxExemptionReasonCode(string taxExemptionReasonCode)
        {
            if (string.IsNullOrWhiteSpace(taxExemptionReasonCode))
                throw new ArgumentException("Código de motivo de isenção é obrigatório.");
            TaxExemptionReasonCode = taxExemptionReasonCode;
        }

        private void SetCategoriaId(int categoriaId)
		{
			if (categoriaId <= 0)
				throw new ArgumentException("Categoria é obrigatória.");

			CategoriaId = categoriaId;
		}

        private void SetPontoDeVendasId(int pontoDeVendasId)
        {
            if (pontoDeVendasId <= 0)
                throw new ArgumentException("Ponto de vendas é obrigatório.");
                
            PontoDeVendasId = pontoDeVendasId;
        }

        private void CalcularValoresDerivados()
        {
            // Calcular lucro unitário
            Lucro = Valor - PrecoCompra;
            
            // Calcular valor fixo (valor com desconto aplicado)
            ValorFixo = CalcularValorFinal();
            
            // Calcular preço com IVA (assumindo 23% de IVA - pode ser configurável)
            var percentualIva = 23f; // Pode vir de configuração
            PrecoCIva = ValorFixo * (1 + percentualIva / 100);
        }

		private void AdicionarMovimentacaoEstoque(int quantidade, string tipo, string observacao)
		{
			var movimentacao = new ProdutoStock
			{
				ProdutoId = this.Id,
				Quantidade = Math.Abs(quantidade),
				QuantidadeMinima = EstoqueMinimo,
				QuantidadeMaxima = int.MaxValue,
				Observacoes = $"{tipo}: {observacao}"
			};
			
			ProdutoStocks.Add(movimentacao);
			
        }

        private void AtualizarDataModificacao()
        {
            LastModifiedDate = DateTime.UtcNow;
        }

        // Métodos estáticos para criação

        /// <summary>
        /// Cria um produto básico com valores mínimos
        /// </summary>
        public static Produto CriarProdutoBasico(
            string nome,
            float valor,
            float precoCompra,
            int categoriaId,
            int pontoDeVendasId,
			string productTypeCode,
			int taxTableEntryId,
			string taxExemptionReasonCode, 
			float margemLucro ,
			int quantidade ,
			int estoqueMinimo ,
			int adicionarStock ,

			float lucro ,
			float valorFixo ,
			float precoCIva ,
			float desconto ,
			float descontoPercentagem ,
			int diasParaExpiracao = 365)
        {
            var dataExpiracao = DateTime.Now.AddDays(diasParaExpiracao);
            return new Produto(nome, valor, precoCompra, categoriaId, pontoDeVendasId,dataExpiracao, productTypeCode,
                                     taxExemptionReasonCode, taxTableEntryId, margemLucro, quantidade, estoqueMinimo, adicionarStock, lucro, valorFixo,
									 precoCIva, desconto,  descontoPercentagem);
        }

        /// <summary>
        /// Cria um produto com estoque inicial
        /// </summary>
        public static Produto CriarProdutoComEstoque(
            string nome,
            float valor,
            float precoCompra,
            int categoriaId,
            int pontoDeVendasId,
            int quantidade,
            int estoqueMinimo,
            string productTypeCode, int taxTableEntryId, string taxExemptionReasonCode,
            int adicionarStock ,
			float lucro ,
			float valorFixo ,
			float precoCIva ,
			float desconto ,
			float descontoPercentagem ,
			int diasParaExpiracao = 365)
        {
            var dataExpiracao = DateTime.Now.AddDays(diasParaExpiracao);
            return new Produto(nome, valor, precoCompra, categoriaId, pontoDeVendasId,
                dataExpiracao,  productTypeCode,  taxExemptionReasonCode, taxTableEntryId,
                margemLucro: 0, quantidade: quantidade, estoqueMinimo: estoqueMinimo,
                adicionarStock, lucro, valorFixo,
                precoCIva, desconto,  descontoPercentagem);
        }
    }
}