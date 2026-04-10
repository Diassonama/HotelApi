using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Hotel.Application.PlanoDeConta.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.PlanoDeConta.Commands
{
    public class DeletePlanoDeContaCommand : IRequest<BaseCommandResponse>
    {
        public int Id { get; set; }

        public class DeletePlanoDeContaCommandHandler : IRequestHandler<DeletePlanoDeContaCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public DeletePlanoDeContaCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(DeletePlanoDeContaCommand request, CancellationToken cancellationToken)
            {
                BaseCommandResponse response = new BaseCommandResponse();

                try
                {
                    var planoDeConta = await _unitOfWork.PlanoDeConta.Get(request.Id);
                    
                    if (planoDeConta == null)
                    {
                        response.Success = false;
                        response.Message = "❌ Plano de Conta não encontrado";
                        return response;
                    }

                    await _unitOfWork.PlanoDeConta.Delete(planoDeConta);
                    await _unitOfWork.Save();

                    response.Success = true;
                    response.Message = "✅ Plano de Conta excluído com sucesso";
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = "❌ Erro ao excluir Plano de Conta";
                    response.Errors = new List<string> { ex.Message };
                }

                return response;
            }
        }
    }
}
