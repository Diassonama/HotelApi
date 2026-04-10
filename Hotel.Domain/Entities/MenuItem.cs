using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Entities
{
    public class MenuItem
    {
         public int Id { get; set; }
    public string Perfil { get; set; }
    public string Label { get; set; }
    public string Action { get; set; }
    public int? ParentId { get; set; } // FK para auto-relacionamento
    public string SubmenuRef { get; set; }

    // Relação Pai-Filho
    public MenuItem Parent { get; set; }
    public ICollection<MenuItem> Children { get; set; }
    }
}