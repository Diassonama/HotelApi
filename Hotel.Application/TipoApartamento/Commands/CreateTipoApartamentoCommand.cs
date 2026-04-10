using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.Responses;
using Hotel.Application.TipoApartamento.Base;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.TipoApartamento.Commands
{
    public class CreateTipoApartamentoCommand : TipoApartamentoCommandBase
    { }
    public class CreateTipoApartamentoCommandHandler : IRequestHandler<CreateTipoApartamentoCommand, BaseCommandResponse>
    {

        private readonly IUnitOfWork _unitOfWork;
        //  private readonly IMediator _mediator;
        private readonly IValidator<CreateTipoApartamentoCommand> _validator;

        public CreateTipoApartamentoCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateTipoApartamentoCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            //  _mediator = mediator;
        }

        public async Task<BaseCommandResponse> Handle(CreateTipoApartamentoCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();

            var validateResult = await _validator.ValidateAsync(request);
            try
            {

                if (!validateResult.IsValid)
                {
                    response.Success = false;
                    response.Message = "Erros encontrado ao cadastrar tipo de apartamento";
                    response.Errors = validateResult.Errors.Select(o => o.ErrorMessage).ToList();
                }
                else
                {
                    var tipoApartamento = new Domain.Entities.TipoApartamento(request.Descricao, request.ValorDiariaSingle, request.ValorDiariaDouble, request.ValorDiariaTriple, request.ValorDiariaQuadruple, request.ValorUmaHora, request.ValorDuasHora, request.ValorTresHora, request.ValorQuatroHora, request.ValorNoite, request.Segunda, request.Terca, request.Quarta, request.Quinta, request.Sexta, request.Sabado, request.Domingo);

                    await _unitOfWork.TipoApartamento.Add(tipoApartamento);
                    // await _unitOfWork.Save();

                    //               await _mediator.Publish(new ApartamentoCreateNotification(apartamento), cancellationToken);
                    response.Data = tipoApartamento;
                    response.Success = true;
                    response.Message = "Tipo de apartamento cadastrado com sucesso";

                    //await _unitOfWork.GetRepository<Domain.Entities.Apartamentos>().Add( apartamento); 
                };
            }
            catch (Exception ex)
            {
                // Tratamento de exceções
                response.Success = false;
                response.Message = $"Erro ao criar tipo de apartamento: {ex.Message}";
            }
            //throw new NotImplementedException();
            return await Task.FromResult(response);

        }
    }


}