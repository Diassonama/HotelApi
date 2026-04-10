using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Cliente.Base;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;

namespace Hotel.Application.Cliente.Commands
{
    public class UpdateClienteCommand: ClienteCommandBase
    {
        public int Id { get; set; }
        public class UpdateClienteCommandHandler : IRequestHandler<UpdateClienteCommand, BaseCommandResponse>
        {
            private readonly IUnitOfWork _unitOfWork;

            public UpdateClienteCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<BaseCommandResponse> Handle(UpdateClienteCommand request, CancellationToken cancellationToken)
            {
                var response = new BaseCommandResponse();
                var existingApartamento = await _unitOfWork.clientes.Get(request.Id);
                
                if (existingApartamento is null)
                {
                    response.Message = "Registro não encontrado";
                    response.Success = false;
                    return response;
                }
try
                {
                    
               
                 var cliente = new Domain.Entities.Cliente(request.Id,request.Nome,request.Email,request.Generos,request.DataAniversario,request.Telefone,request.EmpresasId, request.PaisId);


                await _unitOfWork.clientes.Update(cliente);
              //  await _unitOfWork.Save();

                response.Data = cliente;
                response.Success = true;
                response.Message = "clientes atualizado com sucesso";
              //  return response;


               }
               catch(Exception ex){
                response.Success= false;
                response.Message =  $"Erro ao atualizar registo {ex.Message}";  
               }
                return  await Task.FromResult(response);
            }
        }

    }
}