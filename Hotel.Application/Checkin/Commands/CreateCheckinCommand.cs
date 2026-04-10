using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.Checkin.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Checkin.Commands
{
    public class CreateCheckinCommand: CheckinCommandBase
    {


        public class CreateCheckinCommandHandler : IRequestHandler<CreateCheckinCommand, BaseCommandResponse>
        {
            private IUnitOfWork _unitOfWork;
            private readonly IValidator<CreateCheckinCommand> _validator;
         

            public CreateCheckinCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateCheckinCommand> validator)
            {
                _unitOfWork = unitOfWork;
                _validator = validator;

            }

            public async Task<BaseCommandResponse> Handle(CreateCheckinCommand request, CancellationToken cancellationToken)
            {
                 var response = new BaseCommandResponse();

                var validateResult = await _validator.ValidateAsync(request);

                if(!validateResult.IsValid )
                 {
                    response.Success = false;
                    response.Message = "Erros encontrado ao cadastrar checkin";
                    response.Errors = validateResult.Errors.Select(o=>o.ErrorMessage).ToList();
                 }else
                {
                var checkin = new Domain.Entities.Checkins( request.DataEntrada,request.ValorTotalDiaria);
                //    checkin.Activate();
                await _unitOfWork.checkins.Add(checkin);
              //  await _unitOfWork.Save();
              
                

                
              //  await _mediator.Publish(new ApartamentoCreateNotification(checkin), cancellationToken);
                response.Data = checkin;
                response.Success = true;
                response.Message = "checkin cadastrado com sucesso";
                };         
                //throw new NotImplementedException();
                return  await Task.FromResult(response);
            }
        }
    }
}