using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Domain.Common;
using Hotel.Domain.Enums;

namespace Hotel.Domain.Entities
{
    public class Cliente : BaseDomainEntity
    {
        public string Nome { get; set; }
        public string Telefone { get; set; }
        public string Celular { get; set; }
        public DateTime DataAniversario { get; set; }
        // public string IdGenero { get; set; }
        public Genero Generos { get; set; }
        public string Observacao { get; set; }
        public int EmpresasId { get; set; }
        public string Profissao { get; set; }
        public int PercetualDesconto { get; set; }
        public int PaisId { get; set; }
        public string Endereco { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Complemento { get; set; }
        public string Email { get; set; }
        public string Preferencias { get; set; }
        public string Idioma { get; set; }
        public string HoraParaAcordar { get; set; }
        public string HoraParaDormir { get; set; }

        public string Cargo { get; set; }

        public ICollection<Hospede> Hospedes { get; set; }
       // public ICollection<ApartamentosReservado> ApartamentosReservados { get; set; }
      //  public virtual ICollection<ApartamentosReservado> ApartamentosReservados { get; set; } = new List<ApartamentosReservado>();
       

    // ... other properties
    public virtual ICollection<ApartamentosReservado> ApartamentosReservados { get; set; }

        public ICollection<Lavandaria> Lavandarias { get; set; }
        public Empresa Empresa { get; set; }
        //[ForeignKey("IdPais")]
        public Pais Paises { get; set; }

        public Cliente()
        {

        }
        public Cliente(string nome, string email, Genero genero, DateTime dataAniversario, string telefone, int idEmpresa, int paisId)
        {
            Nome = nome;
            Email = email;
            PercetualDesconto = 0;
            DataAniversario = dataAniversario; //new DateTime(1990, 5, 15);
            Generos = genero;
            Telefone = telefone;
            DateCreated = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            EmpresasId = idEmpresa;
            PaisId = paisId;
            IsActive = true;

            //  SetDesconto(percentualDesconto);
        }


        public Cliente(int id ,string nome, string email, Genero genero, DateTime dataAniversario, string telefone, int idEmpresa, int paisId)
        {
            Id = id;
            Nome = nome;
            Email = email;
            PercetualDesconto = 0;
            DataAniversario = dataAniversario; //new DateTime(1990, 5, 15);
            Generos = genero;
            Telefone = telefone;
            DateCreated = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            EmpresasId = idEmpresa;
            PaisId = paisId;
            IsActive = true;

            //  SetDesconto(percentualDesconto);
        }

        // Método para aplicar desconto
        public void SetDesconto(int percentual)
        {
            if (percentual < 0 || percentual > 100)
                throw new ArgumentException("O percentual de desconto deve estar entre 0 e 100.");
            PercetualDesconto = percentual;
        }

        // Método para atualizar informações pessoais
        public void AtualizarContato(string telefone, string celular, string email)
        {
            Telefone = telefone;
            Celular = celular;
            Email = email;
        }

        // Método para atualizar endereço
        public void AtualizarEndereco(string endereco, string bairro, string cidade, string complemento)
        {
            Endereco = endereco;
            Bairro = bairro;
            Cidade = cidade;
            Complemento = complemento;
        }

        // Método de validação de dados do cliente (exemplo)
        public bool ValidarDados()
        {
            return !string.IsNullOrWhiteSpace(Nome) && !string.IsNullOrWhiteSpace(Email);
        }

    }
}