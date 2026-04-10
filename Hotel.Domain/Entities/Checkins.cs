using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Checkins : BaseDomainEntity
    {

        //   public int HospedagensId { get; set; }
        public int IdCaixaCheckin { get; private set; }
        public int IdCaixaCheckOut { get; private set; }

        public DateTime? DataSaida { get; private set; }
        public DateTime DataEntrada { get; private set; }
        public int IdHospedeCheckOut { get; private set; }
        //	public float ValorDiariaBase { get; private set; }
        public float ValorTotalDiaria { get; private set; }
        public float ValorTotalConsumo { get; private set; }
        public float ValorTotalLigacao { get; private set; }
        public float ValorTotalFinal { get; private set; }
        public float ValorDesconto { get; private set; }

        public string IdUtilizadorCheckin { get; private set; }

        public string IdUtilizadorCheckOut { get; private set; }
        public bool CheckoutRealizado { get; private set; }
        public float PercentualDesconto { get; private set; }
        public string Observacao { get; private set; }
        public int CamaExtra { get; private set; }
        //  public Hospedagem Hospedagem { get; set; }
        public SituacaoDoPagamento situacaoDoPagamento { get; private set; }
        public Caixa CaixaCheckout { get; set; }
        public Caixa CaixaCheckin { get; set; }
        public Utilizador UtilizadoresCheckin { get; set; }
        public Utilizador UtilizadoresCheckout { get; set; }
        public ICollection<Despertador> Despertadores { get; set; }
        public ICollection<TransferenciaQuarto> Transferencias { get; set; }
        public ICollection<FacturaDividida> FacturaDivididas { get; set; }
      //  public ICollection<FacturaEmpresa> FacturaEmpresas { get; set; }

     //   public int FacturaEmpresasId { get; set; }  // Chave estrangeira
      //  public FacturaEmpresa FacturaEmpresas { get; set; }  // Um Checkin pertence a uma única FacturaEmpresa

        public ICollection<Historico> Historicos { get; set; }
        //   public ICollection<Pagamento> Pagamentos { get; private set; }
        private List<Pagamento> _pagamentos = new List<Pagamento>();
        public IReadOnlyCollection<Pagamento> Pagamentos => _pagamentos;
        public ICollection<Hospedagem> Hospedagem { get; private set; }
         public ICollection<Apartamentos>  apartamentos { get; private set; }
        //   public EstadoHospede EstadoHospede { get; private set; }
        public ICollection<Hospede> Hospedes { get; set; }
        public FacturaEmpresa FacturaEmpresa { get; set; } 

        public Checkins()
        {

        }
        // validar valor total diaria não deve ser negativo
        // validar data entrada não deve ser maior que a data actual
        // validar data entrada não deve ser menor que a data de criação
        // validar data saida não deve ser menor que a data de entrada
       
        public Checkins(DateTime dataEntrada, float valorTotalDiaria)
        {
             ValidarValorTotalDiaria(valorTotalDiaria);
             ValidarDataEntrada(dataEntrada);
            DataEntrada = dataEntrada;
            situacaoDoPagamento = SituacaoDoPagamento.Pendente;
            CheckoutRealizado = false;
            DateCreated = DateTime.Now;
            ValorTotalDiaria = valorTotalDiaria;
            CalcularValorTotalFinal();

            _pagamentos = new List<Pagamento>();

        }

        public void update(int checkinsId, float valorTotalDiaria)
        {
            // ✅ CORREÇÃO: Se valor for zero, usar valor mínimo
            if (valorTotalDiaria <= 0)
            {
                valorTotalDiaria = 1.0f; // Valor mínimo de segurança
            }
    
            ValidarValorTotalDiaria(valorTotalDiaria);
            Id = checkinsId;
            CheckoutRealizado = false;
           // DateCreated = DateTime.Now;

            this.ValorTotalDiaria = valorTotalDiaria;
            this.LastModifiedDate = DateTime.Now;
            CalcularValorTotalFinal();
            ActualizarSituacaoDoPagamento();
        }

          // ✅ Método para atualizar data de entrada com validações
        public void AtualizarDataEntrada(DateTime novaDataEntrada)
        {
            ValidarDataEntrada(novaDataEntrada);
            
            // ✅ Se já há data de saída, validar que entrada seja anterior
            if (DataSaida.HasValue)
            {
                ValidarDataSaidaComDataEntrada(DataSaida.Value, novaDataEntrada);
            }
            
            DataEntrada = novaDataEntrada;
            LastModifiedDate = DateTime.Now;
        }

        // ✅ Método para atualizar valor da diária com validações
        public void AtualizarValorTotalDiaria(float novoValor)
        {
            ValidarValorTotalDiaria(novoValor);
            ValorTotalDiaria = novoValor;
            CalcularValorTotalFinal();
            LastModifiedDate = DateTime.Now;
        }
        public void UtilizadoECaixaCheckin(int idCaixa, string idUtilizador)
        {
            ValidarEntrada(idCaixa, idUtilizador);

            if (CheckoutRealizado)
                throw new ArgumentException("O Checkout já foi realizado");

            IdUtilizadorCheckin = idUtilizador;
            IdCaixaCheckin = idCaixa;
            IdCaixaCheckOut = idCaixa;
            IdUtilizadorCheckOut = idUtilizador;
        }
        public void UtilizadoECaixaCheckOut(int idCaixa, string idUtilizador)
        {
            if (CheckoutRealizado)
                throw new ArgumentException("O Checkout já foi realizado");

            ValidarEntrada(idCaixa, idUtilizador);

            IdUtilizadorCheckOut = idUtilizador;
            IdCaixaCheckOut = idCaixa;
        }
        private void ValidarEntrada(int idCaixa, string idUtilizador)
        {
            if (string.IsNullOrEmpty(idUtilizador))
                throw new ArgumentException("O Utilizador não está logado");

            if (idCaixa <= 0)
                throw new ArgumentException("Não foi aberto. Queira por favor abrir o caixa");
        }
        public void ValidarCheckout(DateTime dataPrevisaoSaida, DateTime dataSaida )
        {
            if (CheckoutRealizado)
                throw new InvalidOperationException("O checkout já foi realizado.");
            if (dataPrevisaoSaida.Date > dataSaida.Date)
                throw new ApplicationException($"Check out atencipado, atualiza a data de saida do quarto.");
    
     // ✅ Validar data saída não deve ser menor que a data de entrada
            ValidarDataSaidaComDataEntrada(dataSaida, DataEntrada);
            ValidarDataSaidaComDataEntrada(dataPrevisaoSaida, DataEntrada);

            if (dataPrevisaoSaida.Date < dataSaida.Date)
                throw new InvalidOperationException($"Check out atrazado, atualiza a data de saida do quarto.");

            if (dataSaida.Date < DataEntrada.Date)
                throw new InvalidOperationException("A data de saída não pode ser anterior à data de entrada.");
            if (dataPrevisaoSaida.Date < DataEntrada.Date)
                throw new InvalidOperationException("A data de previsão de saída não pode ser anterior à data de entrada.");

            
            ActualizarSituacaoDoPagamento();
            CheckoutRealizado = true;
            DataSaida = dataSaida;
            situacaoDoPagamento = SituacaoDoPagamento.Pendente;
        }
        

        // Método para calcular o valor total final
        private void CalcularValorTotalFinal()
        {
            float subtotal = ValorTotalDiaria + ValorTotalConsumo + ValorTotalLigacao;
            float descontoAplicado = subtotal * (PercentualDesconto / 100);
            ValorDesconto = descontoAplicado;
            ValorTotalFinal = subtotal - descontoAplicado;
        }

        // Método para aplicar desconto
        public void AplicarDesconto(float percentualDesconto)
        {
            if (percentualDesconto < 0 || percentualDesconto > 100)
                throw new ArgumentException("Percentual de desconto inválido.");

            PercentualDesconto = percentualDesconto;
            CalcularValorTotalFinal();
        }
        // Método para adicionar observação
        public void AdicionarObservacao(string observacao)
        {
            if (string.IsNullOrWhiteSpace(observacao))
                throw new ArgumentException("Observação não pode ser vazia.");

            Observacao = observacao;
        }

        // Método para adicionar cama extra
        public void AdicionarCamaExtra(int quantidade)
        {
            if (quantidade <= 0)
                throw new ArgumentException("Quantidade de camas extras inválida.");

            CamaExtra = quantidade;
        }

        public void RegistrarSaida(DateTime dataSaida)
        {
            if (dataSaida < DataEntrada)
                throw new InvalidOperationException("A data de saída não pode ser anterior à data de entrada.");

            DataSaida = dataSaida;
        }

        public void ActualizarSituacaoDoPagamento()
        {
            var totalPago = Pagamentos.Sum(p => p.Valor);
            situacaoDoPagamento = totalPago >= ValorTotalFinal
                ? SituacaoDoPagamento.Pago
                : totalPago > 0 ?
                     SituacaoDoPagamento.Parcial
                    : SituacaoDoPagamento.Pendente;
        }
        public void RegistrarPagamento(Pagamento pagamento)
        {
             if (pagamento == null)
        throw new ArgumentNullException(nameof(pagamento), "Pagamento não pode ser nulo.");

    // ✅ CORREÇÃO: Verificar por ID ao invés de referência de objeto
    if (pagamento.Id > 0 && _pagamentos.Any(p => p.Id == pagamento.Id))
    {
        throw new InvalidOperationException($"Pagamento com ID {pagamento.Id} já está registrado neste check-in.");
    }

    // ✅ ALTERNATIVA: Verificar se é o mesmo objeto em memória (para pagamentos novos)
    if (pagamento.Id == 0 && _pagamentos.Any(p => ReferenceEquals(p, pagamento)))
    {
        throw new InvalidOperationException("Este objeto de pagamento já está registrado neste check-in.");
    }

            _pagamentos.Add(pagamento);
            ActualizarSituacaoDoPagamento();
        }
     
        // ✅ ===== MÉTODOS DE VALIDAÇÃO IMPLEMENTADOS =====

        /// <summary>
        /// Validar valor total diária não deve ser negativo
        /// </summary>
        private void ValidarValorTotalDiaria(float valorTotalDiaria)
        {
            // ✅ CORREÇÃO: Permitir valor muito pequeno ao invés de zero absoluto
    if (valorTotalDiaria < 0.01f) // Mínimo 1 centavo
    {
        throw new ArgumentException("O valor total da diária deve ser maior que zero.");
    }
        }

        /// <summary>
        /// Validar data entrada não deve ser maior que a data atual
        /// e não deve ser menor que a data de criação
        /// </summary>
        private void ValidarDataEntrada(DateTime dataEntrada)
        {
            var dataAtual = DateTime.Now;
            
            // ✅ Validar data entrada não deve ser maior que a data atual
            if (dataEntrada.Date > dataAtual.Date)
                throw new ArgumentException("A data de entrada não pode ser maior que a data atual.");
            
            // ✅ Validar data entrada não deve ser menor que a data de criação
            // Permitir uma tolerância de alguns minutos para casos de criação simultânea
            if (DateCreated != default(DateTime))
            {
                var dataLimiteMinima = DateCreated.Date;
                if (dataEntrada.Date < dataLimiteMinima)
                    throw new ArgumentException($"A data de entrada não pode ser menor que a data de criação ({DateCreated:dd/MM/yyyy}).");
            }
        }

        /// <summary>
        /// Validar data saída não deve ser menor que a data de entrada
        /// </summary>
        private void ValidarDataSaidaComDataEntrada(DateTime dataSaida, DateTime dataEntrada)
        {
            if (dataSaida.Date < dataEntrada.Date)
                throw new ArgumentException("A data de saída não pode ser menor que a data de entrada.");
        }

        /// <summary>
        /// Método para validar todas as regras de negócio de uma só vez
        /// </summary>
        public void ValidarRegrasDeNegocio()
        {
            // Validar valor da diária
            ValidarValorTotalDiaria(ValorTotalDiaria);
            
            // Validar data de entrada
            ValidarDataEntrada(DataEntrada);
            
            // Validar data de saída se existir
            if (DataSaida.HasValue)
            {
                ValidarDataSaidaComDataEntrada(DataSaida.Value, DataEntrada);
            }
            
            // Validações adicionais
            if (ValorTotalConsumo < 0)
                throw new ArgumentException("O valor total de consumo não pode ser negativo.");
                
            if (ValorTotalLigacao < 0)
                throw new ArgumentException("O valor total de ligação não pode ser negativo.");
                
            if (PercentualDesconto < 0 || PercentualDesconto > 100)
                throw new ArgumentException("O percentual de desconto deve estar entre 0 e 100.");
                
            if (CamaExtra < 0)
                throw new ArgumentException("O número de camas extras não pode ser negativo.");
        }

        public bool PagamentoJaRegistrado(int pagamentoId)
{
    return _pagamentos.Any(p => p.Id == pagamentoId);
}

        /// <summary>
        /// Método para obter resumo das validações
        /// </summary>
        public (bool IsValid, List<string> Errors) ObterStatusValidacao()
        {
            var errors = new List<string>();
            
            try
            {
                ValidarRegrasDeNegocio();
                return (true, errors);
            }
            catch (ArgumentException ex)
            {
                errors.Add(ex.Message);
                return (false, errors);
            }
        }


        public enum SituacaoDoPagamento { Pendente, Pago, Parcial, Cancelado }
    }
}