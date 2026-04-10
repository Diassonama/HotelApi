using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Hotel.Application.Apartamento.Commands.Notifications
{
    public class ApartamentoCreateNotification:INotification
    {
        public Domain.Entities.Apartamentos apartamentos  { get; }

        public ApartamentoCreateNotification(Domain.Entities.Apartamentos apartamentos)
        {
            this.apartamentos = apartamentos;
        }
    }
}