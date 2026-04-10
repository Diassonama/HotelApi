using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hotel.Application.Responses;
using MediatR;

namespace Hotel.Application.MenuRole.Base
{
    public class MenuRoleCommandBase: IRequest<BaseCommandResponse>
    {
        public int MenuId { get; set; }
        public string RoleId { get; set; }
    }
}