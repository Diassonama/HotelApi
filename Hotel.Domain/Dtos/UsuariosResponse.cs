using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Dtos
{
    public class UsuariosResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string  FirstName { get; set; }
        public string LastName { get; set; }
        public List<RoleResponse> UserRoles { get; set; }
    }
}