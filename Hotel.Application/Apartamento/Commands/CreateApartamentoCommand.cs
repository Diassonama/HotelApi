using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Core;
using FluentValidation;
using Hotel.Application.Apartamento.Commands.Notifications;
using Hotel.Application.Apartamentos.Base;
using Hotel.Application.DTOs;
using Hotel.Application.Responses;
using Hotel.Domain.Entities;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Apartamentos.Commands
{
    public class CreateApartamentoCommand: IRequest<BaseCommandResponse>
    {
         public string Codigo { get; set; }
          public int TipoApartamentosId { get; set; }
        }
        public class CreateApartamentoCommandHandler : IRequestHandler< CreateApartamentoCommand , BaseCommandResponse>
   //     public class CreateLeaveAllocationCommandHandler : IRequestHandler<CreateApartamentoCommand, BaseCommandResponse>

        {
            private readonly IApartamentoRepository _apartamentoRepository;
            private readonly IMapper _mapper;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IValidator<CreateApartamentoCommand> _validator;
            private readonly IMediator _mediator;

        public CreateApartamentoCommandHandler(IApartamentoRepository apartamentoRepository, IUnitOfWork unitOfWork, IValidator<CreateApartamentoCommand> validator, IMapper mapper, IMediator mediator)
        {
            _apartamentoRepository = apartamentoRepository;
            //    this.mapper = mapper;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<BaseCommandResponse> Handle(CreateApartamentoCommand request, CancellationToken cancellationToken)
            {
               var response = new BaseCommandResponse();

                var validateResult = await _validator.ValidateAsync(request);

                if(!validateResult.IsValid )
                 {
                    response.Success = false;
                    response.Message = "Erros encontrado ao cadastrar apartamento";
                    response.Errors = validateResult.Errors.Select(o=>o.ErrorMessage).ToList();
                 }else
                {
                var apartamento = new Domain.Entities.Apartamentos(request.Codigo,request.TipoApartamentosId);
                   // apartamento.Activate();
                await _unitOfWork.Apartamento.Add(apartamento);
              //  await _unitOfWork.Save();
                
                await _mediator.Publish(new ApartamentoCreateNotification(apartamento), cancellationToken);
                response.Data = apartamento;
                response.Success = true;
                response.Message = "Apartamento cadastrado com sucesso";
                };         
                //throw new NotImplementedException();
                return  await Task.FromResult(response);
            }
        }
    
}