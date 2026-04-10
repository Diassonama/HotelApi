using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.Caixa.Base;
using Hotel.Application.Extensions;
using Hotel.Application.Responses;
using Hotel.Application.Services;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Caixa.Commands
{
    public class CreateCaixaCommand: IRequest<BaseCommandResponse>
    {
         public float SaldoInicial { get; set; }
        public class CreateCaixaCommandHandler : IRequestHandler<CreateCaixaCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IValidator<CreateCaixaCommand> _validator;
            private readonly UsuarioLogado _usuariologado;

            public CreateCaixaCommandHandler(IUnitOfWork unitOfWork, IValidator<CreateCaixaCommand> validator, UsuarioLogado usuariologado)
            {
                _unitOfWork = unitOfWork;
                _validator = validator;
                _usuariologado = usuariologado;
            }

            public async Task<BaseCommandResponse> Handle(CreateCaixaCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();

                   var validateResult = await _validator.ValidateAsync(request);

                if(!validateResult.IsValid )
                 {
                    response.Success = false;
                    response.Message = "Erros encontrado ao cadastrar caixa";
                    response.Errors = validateResult.Errors.Select(o=>o.ErrorMessage).ToList();
                 }else
                {

               var usrId = ClaimsPrincipalExtensions.GetUserId; //     _usuariologado.UserId;


                var caixa = new Domain.Entities.Caixa(request.SaldoInicial,_usuariologado.IdUtilizador);
                    
                await _unitOfWork.caixa.Add(caixa);
             //   await _unitOfWork.Save();
                
                response.Data = caixa;
                response.Success = true;
                response.Message = "caixa cadastrado com sucesso";
                };         
                //throw new NotImplementedException();
                return  await Task.FromResult(response);

            }
        }

    }
}