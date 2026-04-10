using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Roles.Queries
{
    public class GetRoleByIdQuery : IRequest<BaseCommandResponse>
    {
        public string Id { get; set; }
        public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, BaseCommandResponse>
        {
            private readonly IRoleRepository _repository;

            public GetRoleByIdQueryHandler(IRoleRepository repository)
            {
                _repository = repository;
            }

            public async Task<BaseCommandResponse> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
            {
                var resposta = new BaseCommandResponse();
                var role = await _repository.GetByIdAsync(request.Id);
                if (role != null)
                {
                    resposta.Data = role;
                    resposta.Message = "Role encontrado com sucesso";
                    resposta.Success = true;
                    return resposta;
                }
                else
                {
                    resposta.Success = false;
                    resposta.Message = "Role não encontrado";
                    return resposta;
                }
            }
        }
    }
}