using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;
using static Hotel.Domain.Entities.Checkins;

namespace Hotel.Domain.Entities
{
    public class Hospede : BaseDomainEntity
    {
        public Hospede()
        {
            
        }
        public Hospede(int clientesId, int checkinsId, EstadoHospede estado)
        {
            ClientesId = clientesId;
            CheckinsId = checkinsId;
            Estado = estado;
        }

        public int ClientesId { get; set; }
        public int CheckinsId { get; set; }
        public EstadoHospede Estado { get; private set; }
        public Cliente Clientes { get; set; }
        public Checkins checkins { get; set; }
        public ICollection<Pedido> Pedidos { get; set; }
        public ICollection<Pagamento> Pagamentos { get; set; }

        public enum EstadoHospede { ContaPropria, Empresa }

         public bool PodeFazerCheckout(SituacaoDoPagamento situacaoPagamento)
    {
        if (Estado != EstadoHospede.ContaPropria)
            return true;

        return situacaoPagamento == SituacaoDoPagamento.Pago;
    }






    }
}

public enum EstadoHospede
{
    ContaPropria,
    Empresa
}