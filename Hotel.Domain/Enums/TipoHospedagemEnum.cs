namespace Hotel.Domain.Enums
{
    /// <summary>
    /// Tipos de hospedagem disponíveis
    /// </summary>
    public enum TipoHospedagemEnum
    {
        /// <summary>
        /// Hospedagem por diária completa
        /// </summary>
        DIARIA = 1,

        /// <summary>
        /// Hospedagem por horas
        /// </summary>
        HORA = 2,

        /// <summary>
        /// Hospedagem noturna
        /// </summary>
        NOITE = 3,

        /// <summary>
        /// Hospedagem com valores especiais por dia da semana
        /// </summary>
        ESPECIAL = 4
    }
}