namespace Hotel.Application.DTOs.Response;

public class UsuarioCadastroResponse
{
      public string UsuarioId { get; set; }
       public bool Sucesso { get; private set; }
        public List<string> Erros { get; private set; }

        public UsuarioCadastroResponse() =>
            Erros = new List<string>();

        public UsuarioCadastroResponse(bool sucesso = true) : this() =>
            Sucesso = sucesso;

        public void AdicionarErros(IEnumerable<string> erros) =>
            Erros.AddRange(erros);
}
