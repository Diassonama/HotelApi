using System.ComponentModel.DataAnnotations;
using Hotel.Domain.Enums;

namespace Hotel.Application.DTOs.Request;

public class UsuarioCadastroRequest
{
    /*     [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter entre {8} e {50} caracteres", MinimumLength = 8)]

        public string PrimeroNome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter entre {8} e {50} caracteres", MinimumLength = 8)]

        public string UltimoNome { get; set; } */

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} é inválido")]
    public string Email { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(50, ErrorMessage = "O campo {0} deve ter entre {2} e {1} caracteres", MinimumLength = 5)]
    public string Senha { get; set; }

    /*  [Compare(nameof(Senha), ErrorMessage = "As senhas devem ser iguais")]
     public string SenhaConfirmacao { get; set; } */

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    //[MinLength(5, ErrorMessage = "O campo {0} deve ter entre {8} a {50} caracteres")]
    // public string Usuario { get; set; }

    public string PrimeiroNome { get; set; }
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public string UltimoNome { get; set; }
    public Domain.Enums.Roles Perfil { get; set; }
}
