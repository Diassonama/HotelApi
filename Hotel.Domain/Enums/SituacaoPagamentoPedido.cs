namespace Hotel.Domain.Enums
{
    /// <summary>
    /// Enum para representar as situações de pagamento de um pedido
    /// </summary>
    public enum SituacaoPagamentoPedido
    {
        /// <summary>
        /// Pedido criado mas ainda não pago
        /// </summary>
        Pendente = 1,

        /// <summary>
        /// Pedido pago com sucesso
        /// </summary>
        Pago = 2,

        /// <summary>
        /// Pedido cancelado antes do pagamento
        /// </summary>
        Cancelado = 3,

        /// <summary>
        /// Pagamento foi estornado
        /// </summary>
        Estornado = 4
    }
}