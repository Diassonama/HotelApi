
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using MediatR;

namespace Hotel.Application.Checkin.Base
{
    public class CheckinCommandBase:IRequest<BaseCommandResponse>
    {	
        public int HospedagensId { get; set; }	
		public DateTime DataEntrada { get;  set; }
		public DateTime? DataHoraSaida { get;  set; }
		public int IdHospedeCheckOut { get;  set; }
		public float ValorTotalDiaria { get;  set; }
		public float ValorTotalConsumo { get;  set; }
		public float ValorTotalLigacao { get;  set; }
		public float ValorTotalFinal { get;  set; }
		public float ValorDesconto { get;  set; }
		public float PercentualDesconto { get;  set; }
		public string Observacao { get;  set; }
		public int CamaExtra { get;  set; }
	    
        public Domain.Entities.Hospedagem  Hospedagens  { get;  set; }
    }
}