
using FluentValidation;
using Hotel.Application.Apartamentos.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Apartamento.Commands
{
    public class UpdateApartamentoCommand: IRequest<BaseCommandResponse>
    {

        public int Id { get; set; }
        public string Codigo { get; set; }
        public int TipoApartamentosId { get; set; }
        public class UpdateApartamentoCommandHandler : IRequestHandler<UpdateApartamentoCommand, BaseCommandResponse>
        {
                private readonly IUnitOfWork _unitOfWork;
            //     private readonly IValidator<UpdateApartamentoCommand> _validator;
 //  private readonly IApartamentoRepository _apartamentoRepository;
            public UpdateApartamentoCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            //    _validator = validator;
               // _apartamentoRepository = apartamentoRepository;
            }

            public async Task<BaseCommandResponse> Handle(UpdateApartamentoCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();
                var existingApartamento = await _unitOfWork.Apartamento.Get(request.Id);
                
                if (existingApartamento is null)
                {
                    response.Message = "Registro não encontrado";
                    response.Success = false;
                    return response;
                }

                var apartamento = new Domain.Entities.Apartamentos(request.Id, request.Codigo, request.TipoApartamentosId);

                await _unitOfWork.Apartamento.Update(apartamento);
               // await _unitOfWork.Save();

                response.Data = apartamento;
                response.Success = true;
                response.Message = "Apartamento atualizado com sucesso";
              //  return response;
                return  await Task.FromResult(response);

            }
        }
    }
    
   
}