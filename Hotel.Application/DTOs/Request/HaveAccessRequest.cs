using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.DTOs.Request
{
    public class HaveAccessRequest
    {
        public int MenuId { get; set; }
        public string RoleId { get; set; }
    }
}