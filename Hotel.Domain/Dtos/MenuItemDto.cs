using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Dtos
{
    public class MenuItemDto
    {
         public int Id { get; set; }
    public string Label { get; set; }
    public string Action { get; set; }
    public string SubmenuRef { get; set; }
    public List<MenuItemDto> Children { get; set; }
    }
}