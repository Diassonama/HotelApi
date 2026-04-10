using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Hotel.Application.Apartamentos.Base;
using Hotel.Application.Apartamentos.Commands;

namespace Hotel.Application.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<ApartamentoCommandBase,Domain.Entities.Apartamentos>().ReverseMap();
        }
    }
}