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
    public class UpdatePlanoDeContaCommand : PlanoDeContaCommandBase
    {
        public int Id { get; set; }

        public class UpdatePlanoDeContaCommandHandler : IRequestHandler<UpdatePlanoDeContaCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IValidator<UpdatePlanoDeContaCommand> _validator;
            private readonly IMapper _mapper;

            public UpdatePlanoDeContaCommandHandler(IUnitOfWork unitOfWork, IValidator<UpdatePlanoDeContaCommand> validator, IMapper mapper)
            {
                _unitOfWork = unitOfWork;
                _validator = validator;
                _mapper = mapper;
            }

            public async Task<BaseCommandResponse> Handle(UpdatePlanoDeContaCommand request, CancellationToken cancellationToken)
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
                    var planoDeConta = await _unitOfWork.PlanoDeConta.Get(request.Id);
                    
                    if (planoDeConta == null)
                    {
                        response.Success = false;
                        response.Message = "❌ Plano de Conta não encontrado";
                        return response;
                    }

                    _mapper.Map(request, planoDeConta);
                    
                    await _unitOfWork.PlanoDeConta.Update(planoDeConta);
                    await _unitOfWork.Save();

                    response.Success = true;
                    response.Message = "✅ Plano de Conta atualizado com sucesso";
                    response.Data = new { Id = planoDeConta.Id, Descricao = planoDeConta.Descricao };
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = "❌ Erro ao atualizar Plano de Conta";
                    response.Errors = new List<string> { ex.Message };
                }

                return response;
            }
        }
    }
}
