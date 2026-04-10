using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;

using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Produto.Commands.UpdateProduto
{
    public class UpdateProdutoCommandHandler : IRequestHandler<UpdateProdutoCommand, BaseCommandResponse>
    {
               private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProdutoCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseCommandResponse> Handle(UpdateProdutoCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var validator = new UpdateProdutoCommandValidator();
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.Errors.Count > 0)
            {
                response.Success = false;
                response.Message = "Update Failed";
                response.Errors = validationResult.Errors.Select(q => q.ErrorMessage).ToList();
            }
            else
            {
                var produto = await _unitOfWork.Produto.GetByCodigoAsync(request.Id);

                if (produto is null)
                {
                    response.Success = false;
                    response.Message = $"Produto with Id {request.Id} not found.";
                }
                else
                {
                    _mapper.Map(request, produto, typeof(UpdateProdutoCommand), typeof(Domain.Entities.Produto));

                    await _unitOfWork.Produto.Update(produto);

                    response.Success = true;
                    response.Message = "Update Successful";
                   
                }
            }

            return response;
        }
    }
}