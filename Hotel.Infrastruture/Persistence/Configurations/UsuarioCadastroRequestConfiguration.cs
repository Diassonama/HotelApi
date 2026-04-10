
using Hotel.Application.DTOs.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hotel.Infrastruture.Persistence.Configurations
{
    public class UsuarioCadastroRequestConfiguration : IEntityTypeConfiguration<UsuarioCadastroRequest>
    {
        public void Configure(EntityTypeBuilder<UsuarioCadastroRequest> builder)
        {
          //  builder.Property(u => u.PrimeroNome).HasMaxLength(50).IsRequired();
          //  builder.Property(u => u.UltimoNome).HasMaxLength(50).IsRequired();
            builder.Property(u => u.Email).IsRequired();
            builder.Property(u => u.Senha).HasMaxLength(50).IsRequired();
          //  builder.Property(u => u.Usuario).HasMaxLength(50).IsRequired();
            builder.Property(u => u.Perfil)
            .HasConversion<string>()
            .IsRequired();
            





/*

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(50, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 6)]
    public string Senha { get; set; }

    [Compare(nameof(Senha), ErrorMessage = "As senhas devem ser iguais")]
    public string SenhaConfirmacao { get; set; }

    [Required(ErrorMessage = "O campor {0} é obrigatório")]
    [MinLength(8, ErrorMessage = "O campo {0} deve ter entre {8} a {50} caracteres")]
    public string Usuario { get; set; }
    public Roles Perfil {get; set;} */
        }
    }
}