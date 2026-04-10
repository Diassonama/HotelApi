using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
	public class Param : BaseDomainEntity
	{
		public int Taxa { get; private set; }
		public int CalcularTaxa { get; private set; }
		public int CalcularHora { get; private set; }
		public int Tolerancia { get; private set; }
		public string Regime { get; private set; }
		public string SistemaContabilistico { get; private set; }
		public DateTime DataInicio { get; private set; }
		public DateTime DataFim { get; private set; }
		public string Estabelecimento { get; private set; }
		public string Isencao { get; private set; }
		public int IVA { get; private set; }
		public int RegistroPorPagina { get; private set; }
		public int TipoRecibo { get; private set; }
		public string NomeEmpresa {get; private set;}
		public string Endereco { get; set; }	
		public string Cidade { get; set; }
		public string NumContribuinte { get; set; }
		public string  Telefone { get; set; }
		public string Email { get; set; }
		public string LogoCaminho { get; set; }
		public string ContaBancaria { get; set; }

		public Param()
		{
			
		}

        public Param(int taxa, int calcularTaxa, int calcularHora, int tolerancia, string regime,
                                  string sistemaContabilistico, DateTime dataInicio, DateTime dataFim,
                                  string estabelecimento, string isencao, int iva, int registroPorPagina, int tipoRecibo, 
								  string nomeEmpresa, string endereco, string cidade, string numContribuinte, string telefone, string email, string logoCaminho , string contaBancaria)
        {
            if (taxa < 0) throw new ArgumentException("A taxa não pode ser negativa.");
            if (iva < 0) throw new ArgumentException("O IVA não pode ser negativo.");
            if (dataInicio >= dataFim) throw new ArgumentException("A data de início deve ser anterior à data de fim.");

            Taxa = taxa;
            CalcularTaxa = calcularTaxa;
            CalcularHora = calcularHora;
            Tolerancia = tolerancia;
            Regime = !string.IsNullOrWhiteSpace(regime) ? regime : throw new ArgumentException("O regime é obrigatório.");
            SistemaContabilistico = sistemaContabilistico;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Estabelecimento = !string.IsNullOrWhiteSpace(estabelecimento) ? estabelecimento : throw new ArgumentException("O estabelecimento é obrigatório.");
            Isencao = isencao;
            IVA = iva;
            RegistroPorPagina = registroPorPagina;
            TipoRecibo = tipoRecibo;
            NomeEmpresa = nomeEmpresa;
            Endereco = endereco;
            Cidade = cidade;
            NumContribuinte = numContribuinte;
            Telefone = telefone;
            Email = email;
			LogoCaminho = logoCaminho;
			ContaBancaria = contaBancaria;
        }

        public void AtualizarTaxa(int novaTaxa)
		{
			if (novaTaxa < 0) throw new ArgumentException("A nova taxa não pode ser negativa.");
			Taxa = novaTaxa;
		}

		public void AtualizarPeriodo(DateTime novaDataInicio, DateTime novaDataFim)
		{
			if (novaDataInicio >= novaDataFim) throw new ArgumentException("A data de início deve ser anterior à data de fim.");
			DataInicio = novaDataInicio;
			DataFim = novaDataFim;
		}
		public void Codigo(int id){
			Id = id;
		}

		public bool IsDentroDoPeriodo(DateTime data)
		{
			return data >= DataInicio && data <= DataFim;
		}

		public override string ToString()
		{
			return $"ConfiguracaoFiscal: Taxa={Taxa}, Regime={Regime}, DataInicio={DataInicio.ToShortDateString()}, DataFim={DataFim.ToShortDateString()}";
		}
	}
}