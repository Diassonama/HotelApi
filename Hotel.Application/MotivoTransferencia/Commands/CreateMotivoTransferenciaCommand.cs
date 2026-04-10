using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using Hotel.Domain.Interface.Shared;
using MediatR;

namespace Hotel.Application.MotivoTransferencia.Commands
{
    public class CreateMotivoTransferenciaCommand : IRequest<BaseCommandResponse>
    {
        public string Descricao { get; set; }
       
        public bool Ativo { get; set; } = true;

        public class CreateMotivoTransferenciaCommandHandler : IRequestHandler<CreateMotivoTransferenciaCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public CreateMotivoTransferenciaCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(CreateMotivoTransferenciaCommand request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();

                try
                {
                    // Verificar se código já existe
                    var motivoExistente = await _unitOfWork.MotivoTransferencia.GetByCodigoAsync(request.Descricao);
                    if (motivoExistente != null)
                    {
                        return new BaseCommandResponse
                        {
                            Success = false,
                            Message = "Já existe um motivo com esta Descrição"
                        };
                    }

                    var motivo = new Domain.Entities.MotivoTransferencia(request.Descricao)
                    {
                        IsActive = request.Ativo
                    };

                    await _unitOfWork.MotivoTransferencia.Add(motivo);
                   

                    resposta.Data = motivo;
                    resposta.Success = true;
                    resposta.Message = "Motivo de transferência criado com sucesso";
                }
                catch (Exception ex)
                {
                    resposta.Success = false;
                    resposta.Message = $"Erro ao criar motivo de transferência: {ex.Message}";
                }

                return resposta;
            }
        }
    }
}