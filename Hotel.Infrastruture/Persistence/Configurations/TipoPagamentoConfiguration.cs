using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class TipoPagamentoConfiguration : IEntityTypeConfiguration<TipoPagamento>
    {
        public void Configure(EntityTypeBuilder<TipoPagamento> builder)
        {
           builder.HasKey(p => p.Id);
          //  builder.HasMany(o=>o.Reservas).WithOne(p=>p.TipoPagamentos);
          //  builder.HasMany(o=>o.LancamentoCaixas).WithOne(p=>p.TipoPagamentos);

         
       /*       public ICollection<Pagamento> Pagamentos { get; set; }
        public ICollection<Reserva> Reservas { get; set; }
        public ICollection<LancamentoCaixa> LancamentoCaixas { get; set; } */

        builder.HasData(
                         new TipoPagamento { Id = 1, Descricao = "MultiCaixa",IsActive = true},
                         new TipoPagamento { Id = 2, Descricao = "Cheque bancário", IsActive = true },
                         new TipoPagamento { Id = 3, Descricao = "Compensação de saldos em conta corrente", IsActive = true },
                         new TipoPagamento { Id = 4, Descricao = "Referência de pagamento para Multicaixa", IsActive = true },
                         new TipoPagamento { Id = 5, Descricao = "Numerário", IsActive = true },
                         new TipoPagamento { Id = 6, Descricao = "Transferência bancária", IsActive = true }
                             );
        }
    }
}