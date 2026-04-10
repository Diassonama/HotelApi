using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Roles.Queries
{
    public class GetAllRoleQuery: IRequest<BaseCommandResponse>
    {
        public class GetAllRoleQueryHandler : IRequestHandler<GetAllRoleQuery, BaseCommandResponse>
        {
            private readonly IRoleRepository _repository;

            public GetAllRoleQueryHandler(IRoleRepository repository)
            {
                _repository = repository;
            }

            public async Task<BaseCommandResponse> Handle(GetAllRoleQuery request, CancellationToken cancellationToken)
            {
                 var resposta = new BaseCommandResponse();
                var role = await _repository.GetAllAsync();
                if (role != null && role.Any())
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