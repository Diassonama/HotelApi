using Hotel.Application.Common.Mapping;
using Hotel.Domain.Entities;

namespace Hotel.Application.DTOs;

public class ApartamentoDto //: IMapFrom<Domain.Entities.Apartamentos>
{
        public string Observacao { get; set; }
        public string Situacao { get; set; }
        public int CodigoRamal { get; set; }
        public int CafeDaManha { get; set; }
        public Boolean NaoPertube { get; set; }
        public string Frigobar { get; set; } = "N";
        public Boolean Ativo { get; set; } = false;
        public int TipoGovernancasId { get; set; }
        public int TipoApartamentosId { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
        public int IdTenant { get; set; } = 1;
        public string CreatedBy { get; set; } = "Florindo"; 
        public string LastModifiedBy { get; set; } = "Florindo";
}
