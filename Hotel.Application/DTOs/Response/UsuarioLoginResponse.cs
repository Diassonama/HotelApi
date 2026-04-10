using System.Text.Json.Serialization;

namespace Hotel.Application.DTOs.Response;

public class UsuarioLoginResponse
{
     public bool Sucesso  => Erros.Count == 0;
        
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string AccessToken { get; private set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string RefreshToken { get; private set; }
        public string Role { get; set; }
        public string Usuario { get; set; }
        public string Id { get; set; }
        public List<string> Erros { get; private set; }

        public UsuarioLoginResponse() =>
            Erros = new List<string>();

        public UsuarioLoginResponse(bool sucesso, string accessToken, string refreshToken, string role, string usuario, string id) : this()
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Role = role;
            Usuario = usuario;
            Id = id;
        }

        public void AdicionarErro(string erro) =>
            Erros.Add(erro);

        public void AdicionarErros(IEnumerable<string> erros) =>
            Erros.AddRange(erros);
}
