using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class MotivoTransferencia : BaseDomainEntity
    {
        public string Descricao { get; set; }
        public ICollection<TransferenciaQuarto> Transferencias { get; set; } = new List<TransferenciaQuarto>();
        public ICollection<Transferencia> Transferencia { get; set; } = new List<Transferencia>();
   
   
        public MotivoTransferencia()
        {
            IsActive = true;
            DateCreated = DateTime.UtcNow;
        }

        public MotivoTransferencia(string descricao) : this()
        {
            Descricao = descricao;
            
        }

        public void Atualizar(int id,string descricao, bool ativo)
        {
            Descricao = descricao;
            Id = id;
            IsActive = ativo;
            DateCreated = DateTime.UtcNow;
        }
   
   
   
   
    }

    
}