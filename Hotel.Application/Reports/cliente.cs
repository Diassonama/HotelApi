using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.Reports
{
    public class Cliente
    {
        public int Id{ get; private set; }
        public string Nome { get; private set; }
        public string Endereco { get; private set; }     
        public Cliente(int id, string nome, string endereco)
        {
            if (id < 0)
                throw new InvalidOperationException();
            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(endereco))
                throw new InvalidOperationException();
            Id = id;
            Nome = nome;
            Endereco = endereco;
        }
    }
}