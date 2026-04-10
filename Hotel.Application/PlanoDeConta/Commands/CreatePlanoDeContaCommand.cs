using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.PlanoDeConta.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using AutoMapper;

namespace Hotel.Application.PlanoDeConta.Commands
{
    public class CreatePlanoDeContaCommand : PlanoDeContaCommandBase
    {
        public class CreatePlanoDeContaCommandHandler : IRequestHandler<CreatePlanoDeContaCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IValidator<CreatePlanoDeContaCommand> _validator;
            private readonly IMapper _mapper;

            public CreatePlanoDeContaCommandHandler(IUnitOfWork unitOfWork, IValidator<CreatePlanoDeContaCommand> validator, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<BaseCommandResponse> Handle(CreatePlanoDeContaCommand request, CancellationToken cancellationToken)
            {
                BaseCommandResponse response = new BaseCommandResponse();

                var validateResult = await _validator.ValidateAsync(request);

                if (validateResult.IsValid == false)
                {
                    response.Success = false;
                    response.Message = "❌ Falha na validação dos dados";
                    response.Errors = validateResult.Errors.Select(q => q.ErrorMessage).ToList();
                    return response;
                }

                try
                {
                    var planoDeConta = _mapper.Map<Domain.Entities.PlanoDeConta>(request);
                    
                    await _unitOfWork.PlanoDeConta.Add(planoDeConta);
                    await _unitOfWork.Save();

                    response.Success = true;
                    response.Message = "✅ Plano de Conta criado com sucesso";
                    response.Data = new { Id = planoDeConta.Id, Descricao = planoDeConta.Descricao };
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = "❌ Erro ao criar Plano de Conta";
                    response.Errors = new List<string> { ex.Message };
                }

                return response;
            }
        }
    }
}
