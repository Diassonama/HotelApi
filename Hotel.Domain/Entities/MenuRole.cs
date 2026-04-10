using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Domain.Entities
{
    public class MenuRole
    {
        public int MenuId { get; set; }
        public string RoleId { get; set; }

        public  AppMenu Menu { get; set; }
        public IdentityRole Roles { get; set; }

        public MenuRole(int menuId, string roleId){
            MenuId = menuId;
            RoleId = roleId;
        }
        public MenuRole() { }
    }
}