using System;
using System.ComponentModel;

namespace Hotel.Domain.Enums
{
    /// <summary>
    /// Enum para identificar o tipo de movimento na transferência de quarto
    /// </summary>
    public enum TipoTransferencia
    {
        /// <summary>
        /// Saída do quarto de origem
        /// </summary>
        [Description("Saída")]
        Saida = 1,

        /// <summary>
        /// Entrada no quarto de destino
        /// </summary>
        [Description("Entrada")]
        Entrada = 2
    }
}