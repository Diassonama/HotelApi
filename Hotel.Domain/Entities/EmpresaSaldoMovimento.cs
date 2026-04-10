using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;
using Hotel.Domain.Enums;

namespace Hotel.Domain.Entities
{
    public class EmpresaSaldoMovimento: BaseDomainEntity
    {

        public int EmpresaSaldoId { get; set; }
        public decimal Valor { get; set; }
        public string UtilizadorId { get; set; }
      //  public TipoLancamento TipoLancamentoId { get; set; }
       
        public string Documento { get; set; } // Numero do documento associado ao movimento
        public string Observacao { get; set; }
        public TipoLancamento TipoLancamento { get; set; }
        public  EmpresaSaldo EmpresaSaldo { get; set; } 
         public  Utilizador Utilizador { get; set; }


public EmpresaSaldoMovimento()
        {
        }

        public EmpresaSaldoMovimento(
            int empresaSaldoId, 
            decimal valor, 
            TipoLancamento tipoLancamento, 
           
            string documento, 
            string utilizadorId,
            string observacao = null)
        {
            if (empresaSaldoId <= 0)
                throw new ArgumentException("ID do saldo é obrigatório.");
            if (valor <= 0)
                throw new ArgumentException("O valor deve ser positivo.");
            if (string.IsNullOrWhiteSpace(utilizadorId))
                throw new ArgumentException("Utilizador é obrigatório.");

            EmpresaSaldoId = empresaSaldoId;
            Valor = valor;
            TipoLancamento = tipoLancamento;
         //   TipoLancamentoId = tipoLancamentoId;
            Documento = documento ?? string.Empty;
            UtilizadorId = utilizadorId;
            Observacao = observacao ?? string.Empty;
            DateCreated = DateTime.Now;
        }

        /// <summary>
        /// Atualiza a observação do movimento
        /// </summary>
        public void AtualizarObservacao(string novaObservacao)
        {
            Observacao = novaObservacao ?? string.Empty;
            LastModifiedDate = DateTime.Now;
        }
    }
}