
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Hotel.Domain.Common;
using Hotel.Domain.Enums;
using Flunt.Notifications;

namespace Hotel.Domain.Entities
{
    public class Apartamentos: BaseDomainEntity
    {
        public Apartamentos()
        {
            
        }
        public Apartamentos( int id, string codigo,  int tipoApartamentosId)
        {
             if (id <= 0)
                throw new InvalidOperationException();

            Observacao = "";
            Situacao = Situacao.Livre; // Sempre inicializa como Livre
            CodigoRamal = 0;
            CafeDaManha = 0;
            NaoPertube = true;
            Frigobar = "N";
            TipoGovernancasId = 1; 
            TipoApartamentosId = tipoApartamentosId;
            Codigo = codigo;
            Id  = id;
        }
        public Apartamentos( string codigo,   int tipoApartamentosId)
        {

            if (string.IsNullOrEmpty(codigo) )
                throw new InvalidOperationException();

            Observacao = "";
            Situacao = Situacao.Livre;
            CodigoRamal = 0;
            CafeDaManha = 0;
            NaoPertube = true;
            Frigobar = "N";
            TipoGovernancasId = 1;
            TipoApartamentosId = tipoApartamentosId;
            Codigo = codigo;
            DateCreated = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            IdTenant = 1;
                        
            this.Activate();
        }
        public string Codigo { get; set; }
        public string Observacao { get; set; }
	

    //	public string SituacaoId { get; set; }
		public int CodigoRamal { get; set; }        
		public int CafeDaManha { get; set; }
		public Boolean NaoPertube { get; set; }
		public string Frigobar { get; set; }
		public int TipoGovernancasId { get; set; }
        public int TipoApartamentosId { get; set; } 
        public int? CheckinsId { get; set; }

        [ForeignKey("TipoGovernancasId")]
		public virtual TipoGovernanca TipoGovernancas { get; set; }

        [ForeignKey("TipoApartamentosId")]
		public virtual TipoApartamento TipoApartamentos { get; set; } 
        
        private Situacao _situacao = Situacao.Livre;
        
        /// <summary>
        /// Situação do apartamento. Sempre retorna um valor válido do enum.
        /// Se um valor inválido for definido, automaticamente converte para 'Livre'.
        /// </summary>
        public Situacao Situacao 
        { 
            get => _situacao;
            set 
            {
                // Garante que apenas valores válidos do enum são aceitos
                if (Enum.IsDefined(typeof(Situacao), value))
                {
                    _situacao = value;
                }
                else
                {
                    // Log para debug quando valor inválido é detectado
                    System.Diagnostics.Debug.WriteLine($"Valor inválido para Situacao detectado: {(int)value}. Convertendo para Livre.");
                    _situacao = Situacao.Livre;
                }
            }
        }

        public Checkins  checkins { get; set; }

     //   public ICollection<Checkins> Checkins { get; set; }
     //   public ICollection<Hospedagem> Hospedagems { get; set; }
        public ICollection<Governanca>  Governancas { get; set; }
		public ICollection<ApartamentosReservado> ApartamentosReservados { get; set; }
        public ICollection<MobiliaApartamento> MobiliaApartamentos { get; set; }
        //public ICollection<TransferenciaQuarto> TransferenciaQuartosOrigem { get; set; }
       





        public void liberarApartamento()
        {
            Situacao = Situacao.Livre;
            CheckinsId = null;
        }

        public void ocuparApartamento(int idhospedagem){
          
            Situacao = Situacao.Ocupado;
            CheckinsId = idhospedagem;
        }
        public bool ApartamentoOcupado(bool situacao){
             return situacao;
        }
       public void Activate()
        {
            IsActive = true;
            LastModifiedDate = DateTime.Now;
        }

        public void Inactivate()
        {
            IsActive = false;
            LastModifiedDate = DateTime.Now;
        }
    }
   
}