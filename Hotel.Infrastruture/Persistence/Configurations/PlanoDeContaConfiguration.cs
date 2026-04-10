using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class PlanoDeContaConfiguration : IEntityTypeConfiguration<PlanoDeConta>
    {
        public void Configure(EntityTypeBuilder<PlanoDeConta> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Descricao).HasMaxLength(150);
         //   builder.HasMany(p => p.Produtos).WithOne(p => p.PlanoDeContas).IsRequired();
            builder.HasOne(p => p.Contas).WithMany().IsRequired();

               builder.HasData(
                            new PlanoDeConta { Id = 1, Descricao = "Recebimento Diversos", ContasId =1 },
                            new PlanoDeConta { Id = 2, Descricao = "Venda" , ContasId =1},
                            new PlanoDeConta { Id = 3, Descricao = "Abertura de Caixa", ContasId =1 },
                            new PlanoDeConta { Id = 4, Descricao = "Restaurante" , ContasId =1},
                            new PlanoDeConta { Id = 5, Descricao = "Diarias", ContasId =1 },
                            new PlanoDeConta { Id = 6, Descricao = "Lavandaria", ContasId =1 },
                            new PlanoDeConta { Id = 7, Descricao = "Bar", ContasId =1 },
                            new PlanoDeConta { Id = 8, Descricao = "Telefone" , ContasId =1},
                            new PlanoDeConta { Id = 9, Descricao = "Transporte", ContasId =1 },
                            new PlanoDeConta { Id = 10, Descricao = "Extra Alojamento", ContasId =1 },
                            new PlanoDeConta { Id = 11, Descricao = "Pagamento de Prestação de Serviço", ContasId =1 },
                            new PlanoDeConta { Id = 12, Descricao = "Material de Escritório", ContasId =1 },
                            new PlanoDeConta { Id = 13, Descricao = "Compra de Mercadoria", ContasId =1 },
                            new PlanoDeConta { Id = 14, Descricao = "Pagamentos Diversos" , ContasId =1 }
                             
                            );  

                     
}
}
}
