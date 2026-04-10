using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common.Mapping;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Enums;
using MediatR;

namespace Hotel.Application.Apartamentos.Base
{
    public class ApartamentoCommandBase: IRequest<BaseCommandResponse>
    {
       // public int Id { get; set; }
       public ApartamentoCommandBase()
       {
        
       }
       public string Codigo { get; set; }
        public string Observacao { get; set; }
     //   public Situacao Situacao { get; set; } 
        public int CodigoRamal { get; set; }
        public int CafeDaManha { get; set; }
        public Boolean NaoPertube { get; set; }
        public string Frigobar { get; set; } = "N";
  //      public Boolean IsAtivo { get; set; } = false;
        public int TipoGovernancasId { get; set; }
        public int TipoApartamentosId { get; set; }
     //   public DateTime DateCreated { get; set; } = DateTime.Now;
     //   public DateTime LastModifiedDate { get; set; } = DateTime.Now;
        public int IdTenant { get; set; } = 1;
       // public string CreatedBy { get; set; } = "Florindo"; 
       // public string LastModifiedBy { get; set; } = "Florindo";
    }
}