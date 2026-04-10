
using Hotel.Domain.Common;

namespace Hotel.Domain.Entities
{
    public class Empresa : BaseDomainEntity
    {
        public string RazaoSocial { get; private set; }
        public string Telefone { get; private set; }
        public string Email { get; private set; }
        public float PercentualDesconto { get; private set; }

        public string Endereco { get; private set; }
        public string Bairro { get; private set; }
        public string Cidade { get; private set; }
        public string NumContribuinte { get; set; }
   

        public ICollection<Hospedagem> Hospedagens { get; private set; } = new List<Hospedagem>();
        public ICollection<FacturaEmpresa> FacturaEmpresas { get; private set; } = new List<FacturaEmpresa>();
        public ICollection<Reserva> Reservas { get; private set; } = new List<Reserva>();
        public ICollection<Cliente> Clientes { get; private set; } = new List<Cliente>();
        public ICollection<EmpresaSaldo> EmpresaSaldos { get; private set; } = new List<EmpresaSaldo>();
        public  ICollection<ContaPagar> ContaPagar { get; private set; } 
         public  ICollection<ContaReceber> ContaReceber { get; private set; } 
        
public Empresa()
{
    
}
        public Empresa(string razaoSocial, string telefone, string endereco, string email, float percentual,string numContribuinte = null)
        {
            AtualizarRazaoSocial(razaoSocial);
            AtualizarTelefone(telefone);
            AtualizarEmail(email);
            AtualizarEndereco(endereco);
            AtualizarPercentualDesconto(percentual);
            NumContribuinte = numContribuinte;
        }

        public void AtualizarRazaoSocial(string razaoSocial)
        {
            if (string.IsNullOrWhiteSpace(razaoSocial))
                throw new ArgumentException("Razão Social é obrigatória.");
            RazaoSocial = razaoSocial;
        }
         public void AtualizarEndereco(string endereco)
        {
            if (string.IsNullOrWhiteSpace(endereco))
                throw new ArgumentException("Endereço Social é obrigatória.");
            Endereco = endereco;
        }

        public void AtualizarTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                throw new ArgumentException("Telefone é obrigatório.");
            Telefone = telefone;
        }

        public void AtualizarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("E-mail inválido.");
            Email = email;
        }

        public void Atualiza(int id, string razaoSocial, string email ,string endereco, string bairro, string cidade, string telefone, float percentualDesconto, string numContribuinte)
        {
            Endereco = endereco;
            Bairro = bairro;
            Cidade = cidade;
            RazaoSocial = razaoSocial;
            Email = email;
            Telefone = telefone;
            PercentualDesconto = percentualDesconto;
            Id= id;
            NumContribuinte = numContribuinte;
        }

        public void AtualizarPercentualDesconto(float percentual)
        {
            if (percentual < 0 || percentual > 100)
                throw new ArgumentException("Percentual de desconto deve estar entre 0 e 100.");
            PercentualDesconto = percentual;
        }
    }

}
