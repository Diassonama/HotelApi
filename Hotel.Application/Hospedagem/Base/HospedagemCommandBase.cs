using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using MediatR;

namespace Hotel.Application.Hospedagem.Base
{
    public class HospedagemCommandBase: IRequest<BaseCommandResponse>
    {
        public HospedagemCommandBase()
        {
            PrevisaoFechamento = DateTime.Now.AddDays(1);
        }
   // public string Descricao { get; set; }
   // public bool DiariaAntecipada { get; set; }
    public DateTime DataAbertura { get;  set; }
   // public bool EarlyCheckin { get; set; }
    public DateTime PrevisaoFechamento { get; set; }  = DateTime.Now.AddDays(1);
    public int QuantidadeCrianca { get;  set; }
    public int QuantidadeHomens { get;  set; }
    public int QuantidadeMulheres { get;  set; }
    public int TipoHospedagensId {get; set;}
    public int EmpresasId { get; set; }
    public int MotivoViagensId { get; set; }

   // public int QuantidadeDeDiarias { get; set; }
    public float ValorDiaria { get; set; }
    public int ApartamentosId { get; set; }
    public int IdCliente { get; set; }
  //  public Empresa Empresas { get; set; }
   // public Domain.Entities.Apartamentos Apartamentos { get; set; }
   // public MotivoViagem MotivoViagens { get; set; }
  //  public TipoHospedagem TipoHospedagens { get; set; }

  //  public ICollection<Hospede> Hospedes { get; set; }
 

    }
}