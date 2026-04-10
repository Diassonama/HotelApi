using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.ConfiguracaoFiscal.Base
{
	public class ConfiguracaoFiscalBase : IRequest<BaseCommandResponse>
	{
		public int Taxa { get; set; }
		public int CalcularTaxa { get; set; }
		public int CalcularHora { get; set; }
		public int Tolerancia { get; set; }
		public string Regime { get; set; }
		public string SistemaContabilistico { get; set; }
		public DateTime DataInicio { get; set; }
		public DateTime DataFim { get; set; }
		public string Estabelecimento { get; set; }
		public string Isencao { get; set; }
		public int IVA { get; set; }
		public int RegistroPorPagina { get; set; }
		public int TipoRecibo { get; set; }
		public string NomeEmpresa { get; set; }
		public string Endereco { get; set; }
		public string Cidade { get; set; }
		public string NumContribuinte { get; set; }
		public string Telefone { get; set; }
		public string Email { get; set; }
		public string LogoCaminho { get; set; }
		public string ContaBancaria { get; set; }
	}
}
