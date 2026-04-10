using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Common.Mapping;
using Hotel.Domain.Entities;

namespace Hotel.Application.Apartamento.Base
{
 public class ApartamentoResultModel : IMapFrom<Domain.Entities.Apartamentos> //: ApartamentoCommandBase
    {
        public int Id { get; set; }
        public string Observacao { get; set; }
        public string Situacao { get; set; }
        public int CodigoRamal { get; set; }
        
        public int CafeDaManha { get; set; }
        public Boolean NaoPertube { get; set; }
        public string Frigobar { get; set; }
        public int TipoGovernancasId { get; set; }
        public int TipoApartamentosId { get; set; }

        public TipoGovernanca TipoGovernancas { get; set; }
     //   public TipoApartamento TipoApartamentos { get; set; }

        /*  public void Mapping(Profile profile)
        {
            var c = profile.CreateMap<Apartamento, ApartamentoResultModel>()
                .ForMember(d => d.CafeDaManha, opt => opt.Ignore())
                .ForMember(d => d.Title, opt => opt.NullSubstitute("N/A"))
                .ForMember(d => d.WorkAddress, opt => opt.MapFrom(s => s.OfficeAddress));
        }  */
    }
}