using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Domain.Tenant.Entities
{
    public class CustomSettings
    {
        public string Theme { get; set; }
        public string Colour { get; set; }
        public string Font { get; set; }

        public CustomSettings(string theme, string colour, string font)
        {
            Theme = theme;
            Colour = colour;
            Font = font;
        }
    }
}