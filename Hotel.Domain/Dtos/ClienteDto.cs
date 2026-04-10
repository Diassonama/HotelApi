using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Dtos
{
    public class ClienteDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Documento { get; set; }  // Ex: CPF/CNPJ ou Passaporte
        public string Telefone { get; set; }
        public string Email { get; set; }
    }
}