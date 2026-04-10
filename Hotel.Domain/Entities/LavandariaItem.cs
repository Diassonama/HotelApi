using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
     public class LavandariaItem:BaseDomainEntity
    {
        public int Quantidade { get; set; }
        public float Preco { get; set; }
        public string Cor { get; set; }
        public string Tamanho { get; set; }
        public string Descricao { get; set; }
        public string Observacao { get; set; }
        public DateTime DataEntrega { get; set; }
        public Boolean Entregue { get; set; }

        public Lavandaria Lavandarias { get; set; }
        public Utilizador Utilizadores { get; set; }
    }
}