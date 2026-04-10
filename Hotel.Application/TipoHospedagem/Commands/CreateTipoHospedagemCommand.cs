using Hotel.Application.DTOs.TipoHospedagem;
using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.TipoHospedagem.Commands
{
    /// <summary>
    /// Command para criar um novo tipo de hospedagem
    /// </summary>
    public class CreateTipoHospedagemCommand : IRequest<BaseCommandResponse>
    {
        public string Descricao { get; set; } = string.Empty;
        public float Valor { get; set; }

        public CreateTipoHospedagemCommand() { }

        public CreateTipoHospedagemCommand(CreateTipoHospedagemRequest request)
        {
            Descricao = request.Descricao?.Trim() ?? string.Empty;
            Valor = request.Valor;
        }
    }
}