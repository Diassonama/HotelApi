using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class EmpresaSaldo: BaseDomainEntity
    {
        //public int Id { get; set; }
       /*  public int EmpresaId { get; set; }
        public int Saldo { get; set; }
        public Empresa Empresa { get; set; }
        public ICollection<EmpresaSaldoMovimento> EmpresaSaldoMovimentos { get; set; } 
 */        

   public int EmpresaId { get; private set; }
        public decimal Saldo { get; private set; }
           
        public virtual Empresa Empresa { get; set; }
        public virtual ICollection<EmpresaSaldoMovimento> EmpresaSaldoMovimentos { get; private set; } = new List<EmpresaSaldoMovimento>();

        public EmpresaSaldo()
        {
        }

        public EmpresaSaldo(int empresaId, decimal saldoInicial = 0)
        {
            if (empresaId <= 0)
                throw new ArgumentException("ID da empresa é obrigatório.");
            
            EmpresaId = empresaId;
            Saldo = saldoInicial >= 0 ? saldoInicial : 0;

        }

        /// <summary>
        /// Adiciona saldo à empresa (crédito/adiantamento)
        /// </summary>
        public void AdicionarSaldo(decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("O valor deve ser positivo.");
            
            Saldo += valor;
          
        }

        /// <summary>
        /// Remove saldo da empresa (débito/uso)
        /// </summary>
        public void DebitarSaldo(decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("O valor deve ser positivo.");
            
            if (Saldo < valor)
                throw new InvalidOperationException($"Saldo insuficiente. Saldo atual: {Saldo}, Solicitado: {valor}");
            
            Saldo -= valor;
           
        }

        /// <summary>
        /// Verifica se há saldo suficiente
        /// </summary>
        public bool TemSaldoSuficiente(decimal valor)
        {
            return Saldo >= valor;
        }
    }
}
