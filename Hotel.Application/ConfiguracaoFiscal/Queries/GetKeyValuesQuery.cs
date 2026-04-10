using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using Hotel.Domain.Interface;
using MediatR;
using Hotel.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading;


namespace Hotel.Application.ConfiguracaoFiscal.Queries
{
    public class GetKeyValuesQuery : IRequest<List<AppConfig>>
    {
       public class GetKeyValuesQueryHandler : IRequestHandler<GetKeyValuesQuery, List<AppConfig>>
        {

            private readonly IUnitOfWork  _unitOfWork;
    
            public GetKeyValuesQueryHandler( IUnitOfWork unitOfWork) 
            {       
                _unitOfWork = unitOfWork;

            }
            
            public async Task<List<AppConfig>> Handle(GetKeyValuesQuery request, CancellationToken cancellationToken)
            {
                var registros = await _unitOfWork.AppConfig.GetAllAsync();
                var listaExpandida = new List<AppConfig>();

                foreach (var registro in registros)
                {
                    if (!string.IsNullOrWhiteSpace(registro.Value) && registro.Value.Trim().StartsWith("[") && registro.Value.Trim().EndsWith("]"))
                    {
                        try
                        {
                            var jsonArray = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(registro.Value);
                            if (jsonArray != null)
                            {
                                foreach (var item in jsonArray)
                                {
                                    listaExpandida.Add(new AppConfig
                                    {
                                        Id = int.Parse(item["id"].ToString()),
                                        Key = registro.Key,
                                        Value = item.ContainsKey("Provincia") ? item["Provincia"].ToString() :
                                                JsonSerializer.Serialize(item)
                                    });
                                }
                            }
                        }
                        catch (JsonException)
                        {
                            listaExpandida.Add(new AppConfig { Id = registro.Id, Key = registro.Key, Value = registro.Value });
                        }
                    }
                    else
                    {
                        listaExpandida.Add(new AppConfig { Id = registro.Id, Key = registro.Key, Value = registro.Value });
                    }
                }
                return listaExpandida;
            }
        }
 
    }
}