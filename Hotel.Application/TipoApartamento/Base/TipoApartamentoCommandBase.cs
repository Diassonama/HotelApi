using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.TipoApartamento.Base
{
    public class TipoApartamentoCommandBase : IRequest<BaseCommandResponse>
    {
        public TipoApartamentoCommandBase()
        {
            
        }
        public string Descricao { get;  set; }
       /*  public int QuantidadeCrianca { get; private set; }
        public int QuantidadeHomens { get; private set; }
        public int QuantidadeMulheres { get; private set; } */
        public float ValorDiariaSingle { get;  set; }
        public float ValorDiariaDouble { get;  set; }
        public float ValorDiariaTriple { get;  set; }
        public float ValorDiariaQuadruple { get;  set; }
        public float ValorUmaHora { get;   set; }
        public float ValorDuasHora { get;  set; }
        public float ValorTresHora { get; set; }
        public float ValorQuatroHora { get; set; }
        public float ValorNoite { get; set; }
        public float  Segunda { get;  set; }
        public float Terca { get; set; }
        public float Quarta { get; set; }
        public float Quinta { get; set; }
        public float Sexta { get; set; }
        public float Sabado { get; set; }
        public float Domingo { get; set; } 
    }
}